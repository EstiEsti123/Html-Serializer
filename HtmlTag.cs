//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//// מחלקה לייצוג אלמנט (תגית) HTML
//public class HtmlTag : HtmlNode
//{
//    public override string NodeType => "Tag";

//    /// <summary>שם התגית (למשל: "div", "a")</summary>
//    public string Name { get; set; }

//    /// <summary>מאפיין Id של התגית. אם לא קיים - יהיה null.</summary>
//    public string Id { get; set; }

//    /// <summary>מילון של כל המאפיינים (Attributes) למעט Id ו-Class.</summary>
//    public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

//    /// <summary>רשימה של שמות המחלקות (Classes) המוגדרות לתגית.</summary>
//    public List<string> Classes { get; set; } = new List<string>();

//    /// <summary>רשימת האלמנטים והטקסטים המקוננים בתוך תגית זו.</summary>
//    public List<HtmlNode> Children { get; set; } = new List<HtmlNode>();

//    /// <summary>תוכן ה-HTML הגולמי של האלמנט, כולל התגיות הפנימיות.</summary>
//    public string InnerHtml
//    {
//        get
//        {
//            StringBuilder sb = new StringBuilder();
//            foreach (var child in Children)
//            {
//                if (child is HtmlTag tagChild)
//                {
//                    sb.Append(tagChild.ToOuterHtml());
//                }
//                else if (child is HtmlText textChild)
//                {
//                    sb.Append(textChild.Content);
//                }
//            }
//            return sb.ToString();
//        }
//    }

//    public HtmlTag(string name)
//    {
//        this.Name = name.ToLower();
//    }

//    /// <summary>
//    /// מחזיר את כל ה-HTML של התגית כולל תגיות הפתיחה והסגירה שלה (OuterHtml).
//    /// </summary>
//    public string ToOuterHtml()
//    {
//        StringBuilder sb = new StringBuilder();

//        sb.Append($"<{Name}");

//        if (!string.IsNullOrEmpty(Id))
//        {
//            sb.Append($" id=\"{Id}\"");
//        }

//        if (Classes.Any())
//        {
//            sb.Append($" class=\"{string.Join(" ", Classes)}\"");
//        }

//        foreach (var attr in Attributes)
//        {
//            sb.Append($" {attr.Key}=\"{attr.Value}\"");
//        }

//        sb.Append(">");

//        sb.Append(InnerHtml);

//        // שיפור: לא סוגרים תגיות ריקות
//        if (!IsVoidTag())
//        {
//            sb.Append($"</{Name}>");
//        }

//        return sb.ToString();
//    }

//    public override string ToString() => $"[TAG] <{Name} ID='{Id ?? "None"}' Classes={Classes.Count}> ({Children.Count} Children)";

//    // ******************************************************
//    // פונקציות אחזור (Selection)
//    // ******************************************************

//    /// <summary>
//    /// מוצא את האלמנט הראשון בתוך העץ כולל את עצמו, עם ה-ID המבוקש.
//    /// </summary>
//    public HtmlTag FindElementById(string id)
//    {
//        // בדיקה מיידית: אם ה-ID של הצומת הנוכחי תואם
//        if (this.Id != null && this.Id.Equals(id, StringComparison.OrdinalIgnoreCase))
//        {
//            return this;
//        }

//        // חיפוש רקורסיבי בילדים
//        foreach (var child in Children)
//        {
//            if (child is HtmlTag tagChild)
//            {
//                var found = tagChild.FindElementById(id);
//                if (found != null)
//                {
//                    return found;
//                }
//            }
//        }

//        return null;
//    }

//    /// <summary>
//    /// מוצא את כל האלמנטים בתוך העץ (כולל עצמו) עם שם התגית המבוקש.
//    /// </summary>
//    public List<HtmlTag> FindElementsByTagName(string tagName)
//    {
//        var foundElements = new List<HtmlTag>();
//        string lowerTagName = tagName.ToLower();

//        // בדיקה מיידית: אם הצומת הנוכחי תואם לשם התגית
//        if (this.Name.Equals(lowerTagName, StringComparison.OrdinalIgnoreCase))
//        {
//            foundElements.Add(this);
//        }

//        // חיפוש רקורסיבי בילדים
//        foreach (var child in Children)
//        {
//            if (child is HtmlTag tagChild)
//            {
//                // משתמשים ב-AddRange כדי לצרף את כל התוצאות מהענף הנוכחי
//                foundElements.AddRange(tagChild.FindElementsByTagName(tagName));
//            }
//        }

//        return foundElements;
//    }
//    /// <summary>
//    /// מוצא את כל האלמנטים בתוך העץ (כולל עצמו) המכילים את שם המחלקה המבוקש.
//    /// </summary>
//    public List<HtmlTag> FindElementsByClassName(string className)
//    {
//        var foundElements = new List<HtmlTag>();
//        string lowerClassName = className.ToLower();

//        // 1. בדיקה מיידית: אם הצומת הנוכחי מכיל את המחלקה
//        // משתמשים ב-Contains כדי לבדוק אם הרשימה Classes מכילה את המחלקה המבוקשת.
//        if (this.Classes.Contains(lowerClassName))
//        {
//            foundElements.Add(this);
//        }

//        // 2. חיפוש רקורסיבי בילדים
//        foreach (var child in Children)
//        {
//            if (child is HtmlTag tagChild)
//            {
//                // משתמשים ב-AddRange כדי לצרף את כל התוצאות מהענף הנוכחי
//                foundElements.AddRange(tagChild.FindElementsByClassName(className));
//            }
//        }

//        return foundElements;
//    }

//    // ******************************************************
//    // פונקציית עזר: קביעה אם תגית היא ריקה (Void)
//    // ******************************************************
//    private bool IsVoidTag()
//    {
//        // רשימה קשיחה חלקית של תגיות ריקות בסיסיות (עבור סריאליזציה מחודשת)
//        var voidTags = new[] { "area", "base", "br", "col", "embed", "hr", "img", "input", "link", "meta", "param", "source", "track", "wbr" };
//        return voidTags.Contains(this.Name);
//    }
//}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// מחלקה לייצוג אלמנט (תגית) HTML
public class HtmlTag : HtmlNode
{
    public override string NodeType => "Tag";

    /// <summary>שם התגית (למשל: "div", "a")</summary>
    public string Name { get; set; }

    /// <summary>מאפיין Id של התגית. אם לא קיים - יהיה null.</summary>
    public string Id { get; set; }

    /// <summary>מילון של כל המאפיינים (Attributes) למעט Id ו-Class.</summary>
    public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

    /// <summary>רשימה של שמות המחלקות (Classes) המוגדרות לתגית.</summary>
    public List<string> Classes { get; set; } = new List<string>();

    /// <summary>רשימת האלמנטים והטקסטים המקוננים בתוך תגית זו.</summary>
    public List<HtmlNode> Children { get; set; } = new List<HtmlNode>();

    /// <summary>תוכן ה-HTML הגולמי של האלמנט, כולל התגיות הפנימיות.</summary>
    public string InnerHtml
    {
        get
        {
            StringBuilder sb = new StringBuilder();
            foreach (var child in Children)
            {
                if (child is HtmlTag tagChild)
                {
                    sb.Append(tagChild.ToOuterHtml());
                }
                else if (child is HtmlText textChild)
                {
                    sb.Append(textChild.Content);
                }
            }
            return sb.ToString();
        }
    }

    public HtmlTag(string name)
    {
        this.Name = name.ToLower();
    }

    /// <summary>
    /// מחזיר את כל ה-HTML של התגית כולל תגיות הפתיחה והסגירה שלה (OuterHtml).
    /// </summary>
    public string ToOuterHtml()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append($"<{Name}");

        if (!string.IsNullOrEmpty(Id))
        {
            sb.Append($" id=\"{Id}\"");
        }

        if (Classes.Any())
        {
            sb.Append($" class=\"{string.Join(" ", Classes)}\"");
        }

        foreach (var attr in Attributes)
        {
            sb.Append($" {attr.Key}=\"{attr.Value}\"");
        }

        sb.Append(">");

        sb.Append(InnerHtml);

        // שיפור: לא סוגרים תגיות ריקות
        if (!IsVoidTag())
        {
            sb.Append($"</{Name}>");
        }

        return sb.ToString();
    }

    public override string ToString() => $"[TAG] <{Name} ID='{Id ?? "None"}' Classes={Classes.Count}> ({Children.Count} Children)";

    // ******************************************************
    // פונקציות אחזור (Selection) - מעודכן עם HashSet למניעת כפילויות
    // ******************************************************

    /// <summary>
    /// מוצא את האלמנט הראשון בתוך העץ כולל את עצמו, עם ה-ID המבוקש.
    /// </summary>
    public HtmlTag FindElementById(string id)
    {
        // בדיקה מיידית: אם ה-ID של הצומת הנוכחי תואם
        if (this.Id != null && this.Id.Equals(id, StringComparison.OrdinalIgnoreCase))
        {
            return this;
        }

        // חיפוש רקורסיבי בילדים
        foreach (var child in Children)
        {
            if (child is HtmlTag tagChild)
            {
                var found = tagChild.FindElementById(id);
                if (found != null)
                {
                    return found;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// מוצא את כל האלמנטים בתוך העץ (כולל עצמו) עם שם התגית המבוקש.
    /// *משתמש ב-HashSet למניעת כפילויות.*
    /// </summary>
    public List<HtmlTag> FindElementsByTagName(string tagName)
    {
        // משתמשים ב-HashSet לאיסוף התוצאות ומניעת כפילויות
        var foundElements = new HashSet<HtmlTag>();
        string lowerTagName = tagName.ToLower();

        // מתחילים את החיפוש הרקורסיבי מהצומת הנוכחי
        FindElementsByTagNameRecursive(this, lowerTagName, foundElements);

        // ממירים ל-List ומחזירים
        return foundElements.ToList();
    }

    // פונקציית עזר פרטית לחיפוש רקורסיבי (Tag Name)
    private void FindElementsByTagNameRecursive(HtmlTag currentTag, string lowerTagName, HashSet<HtmlTag> results)
    {
        // 1. בדיקת התנאי על הצומת הנוכחי
        if (currentTag.Name.Equals(lowerTagName, StringComparison.OrdinalIgnoreCase))
        {
            results.Add(currentTag); // HashSet דואג למניעת כפילויות
        }

        // 2. רקורסיה על הילדים
        foreach (var child in currentTag.Children)
        {
            if (child is HtmlTag tagChild)
            {
                FindElementsByTagNameRecursive(tagChild, lowerTagName, results);
            }
        }
    }


    /// <summary>
    /// מוצא את כל האלמנטים בתוך העץ (כולל עצמו) המכילים את שם המחלקה המבוקש.
    /// *משתמש ב-HashSet למניעת כפילויות.*
    /// </summary>
    public List<HtmlTag> FindElementsByClassName(string className)
    {
        // משתמשים ב-HashSet לאיסוף התוצאות ומניעת כפילויות
        var foundElements = new HashSet<HtmlTag>();
        string lowerClassName = className.ToLower();

        // מתחילים את החיפוש הרקורסיבי מהצומת הנוכחי
        FindElementsByClassNameRecursive(this, lowerClassName, foundElements);

        // ממירים ל-List ומחזירים
        return foundElements.ToList();
    }

    // פונקציית עזר פרטית לחיפוש רקורסיבי (Class Name)
    private void FindElementsByClassNameRecursive(HtmlTag currentTag, string lowerClassName, HashSet<HtmlTag> results)
    {
        // 1. בדיקת התנאי על הצומת הנוכחי
        if (currentTag.Classes.Contains(lowerClassName))
        {
            results.Add(currentTag); // HashSet דואג למניעת כפילויות
        }

        // 2. רקורסיה על הילדים
        foreach (var child in currentTag.Children)
        {
            if (child is HtmlTag tagChild)
            {
                FindElementsByClassNameRecursive(tagChild, lowerClassName, results);
            }
        }
    }


    // ******************************************************
    // פונקציית עזר: קביעה אם תגית היא ריקה (Void)
    // ******************************************************
    private bool IsVoidTag()
    {
        // רשימה קשיחה חלקית של תגיות ריקות בסיסיות (עבור סריאליזציה מחודשת)
        var voidTags = new[] { "area", "base", "br", "col", "embed", "hr", "img", "input", "link", "meta", "param", "source", "track", "wbr" };
        return voidTags.Contains(this.Name);
    }
}