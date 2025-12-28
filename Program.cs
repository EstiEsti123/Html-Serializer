
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions; // חשוב: נדרש לבדיקות ה-Selection
using System.IO;
public class Program
{
    public static async Task Main(string[] args)
    {
        string urlToScrape = "https://example.com";

        Console.WriteLine("--- טעינת קבצי עזר (HTML Tags) ---");
        HtmlHelper helper = new HtmlHelper();

        if (helper.AllTags.Length == 0 || helper.VoidTags.Length == 0)
        {
            Console.WriteLine("שגיאה קריטית: לא ניתן לטעון את קובצי התגיות. אנא ודא שהם בתיקיית ההרצה הנכונה.");
            return;
        }

        HtmlFetcher fetcher = new HtmlFetcher();
        string htmlContent = await fetcher.Load(urlToScrape);

        if (!string.IsNullOrEmpty(htmlContent))
        {
            HtmlParser parser = new HtmlParser(helper);

            List<string> cleanedParts = parser.SplitAndCleanHtml(htmlContent);

            HtmlTag rootElement = parser.BuildDomTree(cleanedParts);

            if (rootElement != null)
            {
                Console.WriteLine("\n--- בדיקת פונקציות אחזור (Selection) ---");

                string testClassName = "test-class";
                var elementsByClass = rootElement.FindElementsByClassName(testClassName);
                Console.WriteLine($"\nתוצאת FindElementsByClassName('{testClassName}'): נמצאו {elementsByClass.Count} אלמנטים.");

                var paragraphs = rootElement.FindElementsByTagName("p");
                Console.WriteLine($"\nתוצאת FindElementsByTagName('p'): נמצאו {paragraphs.Count} אלמנטים.");
                foreach (var p in paragraphs)
                {
                    string innerText = Regex.Replace(p.InnerHtml, @"[\r\n\t]", " ").Trim();
                    Console.WriteLine($"  - [TAG] <{p.Name}>. תוכן מקוצר: \"{innerText.Substring(0, Math.Min(50, innerText.Length))}...\"");
                }

                string nonExistentId = "main-container";
                var elementById = rootElement.FindElementById(nonExistentId);
                Console.WriteLine($"\nתוצאת FindElementById('{nonExistentId}'): {elementById?.ToString() ?? "לא נמצא"}");

                Console.WriteLine("---------------------------------------------");
                

                Console.WriteLine("\n--- עץ אובייקטים (DOM Tree) ---");
                PrintTree(rootElement, 0);
            }
        }
        else
        {
            Console.WriteLine("הקריאה נכשלה. לא ניתן להמשיך בבניית העץ.");
        }
    }

    private static void PrintTree(HtmlNode node, int depth)
    {
        string indent = new string(' ', depth * 4);

        Console.WriteLine($"{indent}[{node.NodeType}]: {node.ToString()}");

        if (node is HtmlTag tag)
        {
            foreach (var child in tag.Children)
            {
                PrintTree(child, depth + 1);
            }
        }
    }
}

