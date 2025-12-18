using System.Collections.Generic;

// מחלקת בסיס לייצוג רכיב כלשהו ב-DOM
public abstract class HtmlNode
{
    public abstract string NodeType { get; }

    // מאפיין ליצירת עץ: הפניה לאלמנט האב
    public HtmlTag Parent { get; set; }
}

// ייצוג של טקסט שנמצא בין התגיות
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