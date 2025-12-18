
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
        // כתובת לדוגמה
        string urlToScrape = "https://example.com";

        // 1. יצירת מופע של HtmlHelper וטעינת ה-JSON
        Console.WriteLine("--- טעינת קבצי עזר (HTML Tags) ---");
        HtmlHelper helper = new HtmlHelper();

        // בדיקה קריטית: ודא שקובצי ה-JSON נטענו בהצלחה
        if (helper.AllTags.Length == 0 || helper.VoidTags.Length == 0)
        {
            Console.WriteLine("שגיאה קריטית: לא ניתן לטעון את קובצי התגיות. אנא ודא שהם בתיקיית ההרצה הנכונה.");
            return;
        }

        HtmlFetcher fetcher = new HtmlFetcher();
        string htmlContent = await fetcher.Load(urlToScrape);

        if (!string.IsNullOrEmpty(htmlContent))
        {
            // 2. יצירת מופע של HtmlParser והעברת ה-Helper
            HtmlParser parser = new HtmlParser(helper);

            // 3. פירוק המחרוזת
            List<string> cleanedParts = parser.SplitAndCleanHtml(htmlContent);

            // 4. בניית עץ האובייקטים (DOM Tree)
            HtmlTag rootElement = parser.BuildDomTree(cleanedParts);

            if (rootElement != null)
            {
                // ************************************************************
                // 5. קטע קוד חדש: בדיקת פונקציות האחזור (Selection Demo)
                // ************************************************************
                Console.WriteLine("\n--- בדיקת פונקציות אחזור (Selection) ---");

                // א. חיפוש לפי שם מחלקה (FindElementsByClassName) - הבדיקה החדשה!
                // בודקים מחלקה שאינה קיימת בדף example.com
                string testClassName = "test-class";
                var elementsByClass = rootElement.FindElementsByClassName(testClassName);
                Console.WriteLine($"\nתוצאת FindElementsByClassName('{testClassName}'): נמצאו {elementsByClass.Count} אלמנטים.");

                // ב. חיפוש לפי שם תגית (FindElementsByTagName)
                var paragraphs = rootElement.FindElementsByTagName("p");
                Console.WriteLine($"\nתוצאת FindElementsByTagName('p'): נמצאו {paragraphs.Count} אלמנטים.");
                foreach (var p in paragraphs)
                {
                    // ניקוי הטקסט להצגה בקונסולה
                    string innerText = Regex.Replace(p.InnerHtml, @"[\r\n\t]", " ").Trim();
                    Console.WriteLine($"  - [TAG] <{p.Name}>. תוכן מקוצר: \"{innerText.Substring(0, Math.Min(50, innerText.Length))}...\"");
                }

                // ג. חיפוש לפי ID (FindElementById)
                string nonExistentId = "main-container";
                var elementById = rootElement.FindElementById(nonExistentId);
                Console.WriteLine($"\nתוצאת FindElementById('{nonExistentId}'): {elementById?.ToString() ?? "לא נמצא"}");

                Console.WriteLine("---------------------------------------------");
                // ************************************************************

                Console.WriteLine("\n--- עץ אובייקטים (DOM Tree) ---");
                // קריאה לפונקציית העזר PrintTree
                PrintTree(rootElement, 0);
            }
        }
        else
        {
            Console.WriteLine("הקריאה נכשלה. לא ניתן להמשיך בבניית העץ.");
        }
    }

    /// <summary>
    /// פונקציה רקורסיבית סטטית להדפסת מבנה העץ בצורה מוזחת (Indented).
    /// </summary>
    private static void PrintTree(HtmlNode node, int depth)
    {
        // יצירת רווחים לשם הזחה: 4 רווחים לכל רמת עומק
        string indent = new string(' ', depth * 4);

        // הדפסת סוג הצומת והערך שלו.
        Console.WriteLine($"{indent}[{node.NodeType}]: {node.ToString()}");

        // אם הצומת הוא תגית, עוברים על הילדים שלו
        if (node is HtmlTag tag)
        {
            foreach (var child in tag.Children)
            {
                PrintTree(child, depth + 1);
            }
        }
    }
}
//----------...........אם רוצים שליפת קובץ מקומי
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Linq;
//using System.Text.RegularExpressions;
//using System.IO; // נדרש לקריאת קבצים

//public class Program
//{
//    public static async Task Main(string[] args)
//    {
//        // 1. יצירת מופע של HtmlHelper וטעינת ה-JSON
//        Console.WriteLine("--- טעינת קבצי עזר (HTML Tags) ---");
//        HtmlHelper helper = new HtmlHelper();

//        // בדיקה קריטית: ודא שקובצי ה-JSON נטענו בהצלחה
//        if (helper.AllTags.Length == 0 || helper.VoidTags.Length == 0)
//        {
//            Console.WriteLine("שגיאה קריטית: לא ניתן לטעון את קובצי התגיות. אנא ודא שהם בתיקיית ההרצה הנכונה.");
//            return;
//        }

//        // ************************************************************
//        // שינוי קריטי: קריאת קובץ HTML מקומי לבדיקת כפילויות
//        // ************************************************************
//        string filePath = "test_duplicate.html";
//        string htmlContent = "";

//        try
//        {
//            // קורא את כל תוכן הקובץ (הקובץ חייב להיות בתיקיית ההרצה)
//            htmlContent = File.ReadAllText(filePath);
//            Console.WriteLine($"\n--- נטען תוכן מקומי מ- {filePath} ---");
//        }
//        catch (FileNotFoundException)
//        {
//            // אם הקובץ לא נמצא, נחזור לדף example.com כגיבוי, אך מומלץ ליצור את הקובץ.
//            string urlToScrape = "https://example.com";
//            Console.WriteLine($"\nאזהרה: הקובץ {filePath} לא נמצא. מנסה לטעון מ- {urlToScrape} (בדיקת הכפילויות לא תעבוד).");
//            HtmlFetcher fetcher = new HtmlFetcher();
//            htmlContent = await fetcher.Load(urlToScrape);
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"שגיאת קריאת קובץ: {ex.Message}");
//            return;
//        }
//        // ************************************************************


//        if (!string.IsNullOrEmpty(htmlContent))
//        {
//            // 2. יצירת מופע של HtmlParser והעברת ה-Helper
//            HtmlParser parser = new HtmlParser(helper);

//            // 3. פירוק המחרוזת
//            List<string> cleanedParts = parser.SplitAndCleanHtml(htmlContent);

//            // 4. בניית עץ האובייקטים (DOM Tree)
//            HtmlTag rootElement = parser.BuildDomTree(cleanedParts);

//            if (rootElement != null)
//            {
//                // ************************************************************
//                // 5. בדיקת פונקציות האחזור (Selection Demo)
//                // ************************************************************
//                Console.WriteLine("\n--- בדיקת פונקציות אחזור (Selection) ---");

//                // א. בדיקה קריטית: FindElementsByClassName
//                // אם הקוד קורא את test_duplicate.html, הציפייה היא 2
//                string testClassName = "target-class";
//                var elementsByClass = rootElement.FindElementsByClassName(testClassName);

//                Console.WriteLine($"\nתוצאת FindElementsByClassName('{testClassName}'): נמצאו {elementsByClass.Count} אלמנטים.");
//                if (elementsByClass.Count > 0)
//                {
//                    // מדפיסים דוגמה של אחד האלמנטים
//                    string innerText = Regex.Replace(elementsByClass.First().InnerHtml, @"[\r\n\t]", " ").Trim();
//                    Console.WriteLine($"  - [TAG] <{elementsByClass.First().Name}>. תוכן מקוצר: \"{innerText.Substring(0, Math.Min(50, innerText.Length))}...\"");
//                }


//                // ב. חיפוש לפי שם תגית (P)
//                var paragraphs = rootElement.FindElementsByTagName("p");
//                Console.WriteLine($"\nתוצאת FindElementsByTagName('p'): נמצאו {paragraphs.Count} אלמנטים.");
//                foreach (var p in paragraphs.Take(2)) // מדפיסים רק 2 ראשונים אם יש הרבה
//                {
//                    string innerText = Regex.Replace(p.InnerHtml, @"[\r\n\t]", " ").Trim();
//                    Console.WriteLine($"  - [TAG] <{p.Name}>. תוכן מקוצר: \"{innerText.Substring(0, Math.Min(50, innerText.Length))}...\"");
//                }

//                // ג. חיפוש לפי ID
//                string nonExistentId = "main-container";
//                var elementById = rootElement.FindElementById(nonExistentId);
//                Console.WriteLine($"\nתוצאת FindElementById('{nonExistentId}'): {elementById?.ToString() ?? "לא נמצא"}");

//                Console.WriteLine("---------------------------------------------");
//                // ************************************************************

//                Console.WriteLine("\n--- עץ אובייקטים (DOM Tree) ---");
//                // קריאה לפונקציית העזר PrintTree
//                PrintTree(rootElement, 0);
//            }
//        }
//        else
//        {
//            Console.WriteLine("הקריאה נכשלה. לא ניתן להמשיך בבניית העץ.");
//        }
//    }

//    /// <summary>
//    /// פונקציה רקורסיבית סטטית להדפסת מבנה העץ בצורה מוזחת (Indented).
//    /// </summary>
//    private static void PrintTree(HtmlNode node, int depth)
//    {
//        // יצירת רווחים לשם הזחה: 4 רווחים לכל רמת עומק
//        string indent = new string(' ', depth * 4);

//        // הדפסת סוג הצומת והערך שלו.
//        Console.WriteLine($"{indent}[{node.NodeType}]: {node.ToString()}");

//        // אם הצומת הוא תגית, עוברים על הילדים שלו
//        if (node is HtmlTag tag)
//        {
//            foreach (var child in tag.Children)
//            {
//                PrintTree(child, depth + 1);
//            }
//        }
//    }
//}