
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class HtmlTag : HtmlNode
{
    public override string NodeType => "Tag";

    public string Name { get; set; }

    public string Id { get; set; }

    public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

    public List<string> Classes { get; set; } = new List<string>();

    public List<HtmlNode> Children { get; set; } = new List<HtmlNode>();

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
    
    public HtmlTag FindElementById(string id)
    {
        if (this.Id != null && this.Id.Equals(id, StringComparison.OrdinalIgnoreCase))
        {
            return this;
        }

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


    public List<HtmlTag> FindElementsByTagName(string tagName)
    {
        var foundElements = new HashSet<HtmlTag>();
        string lowerTagName = tagName.ToLower();

        FindElementsByTagNameRecursive(this, lowerTagName, foundElements);

        return foundElements.ToList();
    }

    private void FindElementsByTagNameRecursive(HtmlTag currentTag, string lowerTagName, HashSet<HtmlTag> results)
    {
        if (currentTag.Name.Equals(lowerTagName, StringComparison.OrdinalIgnoreCase))
        {
            results.Add(currentTag); // HashSet דואג למניעת כפילויות
        }

        foreach (var child in currentTag.Children)
        {
            if (child is HtmlTag tagChild)
            {
                FindElementsByTagNameRecursive(tagChild, lowerTagName, results);
            }
        }
    }

    public List<HtmlTag> FindElementsByClassName(string className)
    {
        var foundElements = new HashSet<HtmlTag>();
        string lowerClassName = className.ToLower();

        FindElementsByClassNameRecursive(this, lowerClassName, foundElements);

        return foundElements.ToList();
    }
    private void FindElementsByClassNameRecursive(HtmlTag currentTag, string lowerClassName, HashSet<HtmlTag> results)
    {
        
        if (currentTag.Classes.Contains(lowerClassName))
        {
            results.Add(currentTag); // HashSet דואג למניעת כפילויות
        }

        foreach (var child in currentTag.Children)
        {
            if (child is HtmlTag tagChild)
            {
                FindElementsByClassNameRecursive(tagChild, lowerClassName, results);
            }
        }
    }


  
    private bool IsVoidTag()
    {
        var voidTags = new[] { "area", "base", "br", "col", "embed", "hr", "img", "input", "link", "meta", "param", "source", "track", "wbr" };
        return voidTags.Contains(this.Name);
    }
}
