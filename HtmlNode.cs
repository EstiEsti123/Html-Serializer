using System.Collections.Generic;

public abstract class HtmlNode
{
    public abstract string NodeType { get; }

    public HtmlTag Parent { get; set; }
}
public class HtmlText : HtmlNode
{
    public override string NodeType => "Text";
    public string Content { get; set; }

    public HtmlText(string content)
    {
        Content = content;
    }

    public override string ToString() => $"[TEXT] \"{(Content.Length > 30 ? Content.Substring(0, 30) + "..." : Content)}\"";
}
