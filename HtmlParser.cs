using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System; // נדרש ל-StringSplitOptions

public class HtmlParser
{
    private const string HtmlTagPattern = @"<[^>]+>";

    // מאפיין חדש: מחלקת העזר לניהול תגיות
    private readonly HtmlHelper _htmlHelper;

    // Constructor חדש: מקבל מופע של HtmlHelper
    public HtmlParser(HtmlHelper htmlHelper)
    {
        _htmlHelper = htmlHelper;
    }

    // ******************************************************
    // פונקציה 1: פירוק וניקוי
    // ******************************************************
    public List<string> SplitAndCleanHtml(string htmlContent)
    {
        if (string.IsNullOrEmpty(htmlContent))
        {
            return new List<string>();
        }

        List<string> parts = new List<string>();
        MatchCollection matches = Regex.Matches(htmlContent, HtmlTagPattern);

        int lastIndex = 0;

        foreach (Match match in matches)
        {
            if (match.Index > lastIndex)
            {
                string textBetweenTags = htmlContent.Substring(lastIndex, match.Index - lastIndex);
                string cleanedText = CleanText(textBetweenTags);
                if (!string.IsNullOrEmpty(cleanedText))
                {
                    parts.Add(cleanedText);
                }
            }

            parts.Add(match.Value);

            lastIndex = match.Index + match.Length;
        }

        if (lastIndex < htmlContent.Length)
        {
            string remainingText = htmlContent.Substring(lastIndex);
            string cleanedRemainingText = CleanText(remainingText);
            if (!string.IsNullOrEmpty(cleanedRemainingText))
            {
                parts.Add(cleanedRemainingText);
            }
        }

        return parts;
    }

    // פונקציית עזר: ניקוי רווחים וקווים חדשים
    private string CleanText(string text)
    {
        string cleaned = Regex.Replace(text, @"[\r\n\t]", " ");
        cleaned = Regex.Replace(cleaned, @"\s+", " ");
        return cleaned.Trim();
    }

    // ******************************************************
    // פונקציה חדשה: ParseAttributes
    // ******************************************************
    private void ParseAttributes(string attributesString, HtmlTag tag)
    {
        // ביטוי רגולרי לזיהוי זוגות מפתח-ערך או מאפיינים בוליאניים
        const string AttrRegex = @"(\s*?)(?<key>[a-zA-Z0-9_-]+)(?:=(?<quote>['""]?)(?<value>.*?)\k<quote>)?";

        MatchCollection matches = Regex.Matches(attributesString, AttrRegex);

        foreach (Match match in matches)
        {
            string key = match.Groups["key"].Value.ToLower();
            string value = match.Groups["value"].Value;

            if (string.IsNullOrEmpty(key)) continue;

            // 1. טיפול במאפיינים מיוחדים (Id ו-Class)
            if (key == "id")
            {
                tag.Id = value;
            }
            else if (key == "class")
            {
                // מפצלים את מחרוזת המחלקות לפי רווחים
                tag.Classes = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            // 2. טיפול בכל שאר המאפיינים
            else
            {
                // אם הערך ריק (מאפיין בוליאני), נשמור את המפתח גם כערך
                if (string.IsNullOrEmpty(value) && !match.Groups["quote"].Success)
                {
                    tag.Attributes[key] = key;
                }
                else
                {
                    tag.Attributes[key] = value;
                }
            }
        }
    }


    // ******************************************************
    // פונקציה 2: BuildDomTree (מעודכנת ומושלמת)
    // ******************************************************
    public HtmlTag BuildDomTree(List<string> parts)
    {
        var root = new HtmlTag("root-document");
        var elementStack = new Stack<HtmlTag>();
        elementStack.Push(root);

        foreach (var part in parts)
        {
            if (part.StartsWith("<") && part.EndsWith(">"))
            {
                // זהו חלק של תגית
                if (part.StartsWith("</"))
                {
                    // תגית סגירה (למשל: </body>)
                    string closeTagName = Regex.Replace(part, @"</(\w+)\s*>", "$1", RegexOptions.IgnoreCase);

                    // סגירת האלמנט הנוכחי וטיפוס למעלה, אם התגית תואמת
                    if (elementStack.Count > 1 && elementStack.Peek().Name == closeTagName.ToLower())
                    {
                        elementStack.Pop();
                    }
                }
                else
                {
                    // תגית פתיחה או תגית ריקה
                    var match = Regex.Match(part, @"<(\w+)([^>]*)(\/?)>");
                    if (match.Success)
                    {
                        string tagName = match.Groups[1].Value.ToLower();
                        string attributesString = match.Groups[2].Value.Trim();
                        bool isSelfClosingBySyntax = match.Groups[3].Value == "/";

                        // קביעת אם התגית היא ריקה (Void) על בסיס קובץ ה-JSON
                        bool isVoidTagByDefinition = _htmlHelper.VoidTags.Contains(tagName);

                        var newElement = new HtmlTag(tagName);

                        // 💡 שלב 1: פירוק המאפיינים
                        ParseAttributes(attributesString, newElement);

                        // יצירת קישוריות
                        HtmlTag currentParent = elementStack.Peek();
                        currentParent.Children.Add(newElement);
                        newElement.Parent = currentParent;


                        // 💡 שלב 2: החלטה אם לדחוף לערימה
                        // תגיות ריקות (כמו <br>, <img>) או תגיות שנסגרות עצמית (<tag/>) 
                        // לא צריכות להיכנס לערימה, כי הן לא מכילות תוכן או אלמנטים נוספים.
                        if (!isVoidTagByDefinition && !isSelfClosingBySyntax)
                        {
                            elementStack.Push(newElement);
                        }
                    }
                }
            }
            else
            {
                // זהו חלק של טקסט
                var textNode = new HtmlText(part);
                HtmlTag currentParent = elementStack.Peek();
                currentParent.Children.Add(textNode);
                textNode.Parent = currentParent;
            }
        }

        // נחזיר את הילד הראשון של ה-root-document, שהוא למעשה ה-<html> או ה-<body> הראשון
        // אם יש יותר מאחד, נחזיר את האלמנט הראשי המכיל אותם (למשל, ה-<body> הראשון).
        // במקרים נדירים של HTML פגום, נחזיר את root עצמו.
        HtmlTag documentBody = root.Children.OfType<HtmlTag>().FirstOrDefault(t => t.Name == "html" || t.Name == "body");

        return documentBody ?? root;
    }
}