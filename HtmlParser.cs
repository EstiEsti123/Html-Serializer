using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System;

public class HtmlParser
{
    private const string HtmlTagPattern = @"<[^>]+>";

    private readonly HtmlHelper _htmlHelper;
    
    public HtmlParser(HtmlHelper htmlHelper)
    {
        _htmlHelper = htmlHelper;
    }
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

    private string CleanText(string text)
    {
        string cleaned = Regex.Replace(text, @"[\r\n\t]", " ");
        cleaned = Regex.Replace(cleaned, @"\s+", " ");
        return cleaned.Trim();
    }

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

            if (key == "id")
            {
                tag.Id = value;
            }
            else if (key == "class")
            {
                tag.Classes = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            else
            {
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

    public HtmlTag BuildDomTree(List<string> parts)
    {
        var root = new HtmlTag("root-document");
        var elementStack = new Stack<HtmlTag>();
        elementStack.Push(root);

        foreach (var part in parts)
        {
            if (part.StartsWith("<") && part.EndsWith(">"))
            {
                if (part.StartsWith("</"))
                {
                    string closeTagName = Regex.Replace(part, @"</(\w+)\s*>", "$1", RegexOptions.IgnoreCase);

                    if (elementStack.Count > 1 && elementStack.Peek().Name == closeTagName.ToLower())
                    {
                        elementStack.Pop();
                    }
                }
                else
                {
                    var match = Regex.Match(part, @"<(\w+)([^>]*)(\/?)>");
                    if (match.Success)
                    {
                        string tagName = match.Groups[1].Value.ToLower();
                        string attributesString = match.Groups[2].Value.Trim();
                        bool isSelfClosingBySyntax = match.Groups[3].Value == "/";
                        bool isVoidTagByDefinition = _htmlHelper.VoidTags.Contains(tagName);

                        var newElement = new HtmlTag(tagName);

                        ParseAttributes(attributesString, newElement);
 
                        HtmlTag currentParent = elementStack.Peek();
                        currentParent.Children.Add(newElement);
                        newElement.Parent = currentParent;

                        if (!isVoidTagByDefinition && !isSelfClosingBySyntax)
                        {
                            elementStack.Push(newElement);
                        }
                    }
                }
            }
            else
            {
                var textNode = new HtmlText(part);
                HtmlTag currentParent = elementStack.Peek();
                currentParent.Children.Add(textNode);
                textNode.Parent = currentParent;
            }
        }
        HtmlTag documentBody = root.Children.OfType<HtmlTag>().FirstOrDefault(t => t.Name == "html" || t.Name == "body");

        return documentBody ?? root;
    }
}
