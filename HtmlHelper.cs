using System;
using System.IO;
using System.Text.Json;
using System.Linq;

/// <summary>
/// מחלקת עזר לטעינת וניהול רשימות תגיות HTML, כולל תגיות ריקות (Void Tags).
/// </summary>
public class HtmlHelper
{
    // שמות הקבצים שהועלו לפרויקט
    private const string AllTagsFileName = "HtmlTags.json";
    private const string VoidTagsFileName = "HtmlVoidTags.json";

    /// <summary>
    /// כל התגיות החוקיות ב-HTML.
    /// </summary>
    public string[] AllTags { get; private set; }

    /// <summary>
    /// רשימת התגיות שאינן דורשות תגית סגירה (Void Tags).
    /// </summary>
    public string[] VoidTags { get; private set; }

    public HtmlHelper()
    {
        // קריאה וטעינה של הנתונים ב-Constructor
        LoadHtmlTags();
    }

    /// <summary>
    /// טוען את הנתונים מקבצי ה-JSON לתוך המאפיינים המתאימים.
    /// </summary>
    private void LoadHtmlTags()
    {
        try
        {
            // 1. קריאה ודה-סריאליזציה של כל התגיות
            string allTagsJson = File.ReadAllText(AllTagsFileName);
            AllTags = JsonSerializer.Deserialize<string[]>(allTagsJson)?
                                    .Select(t => t.ToLower()) // המרה לאותיות קטנות לחיפוש עקבי
                                    .ToArray() ?? Array.Empty<string>();

            // 2. קריאה ודה-סריאליזציה של התגיות הריקות
            string voidTagsJson = File.ReadAllText(VoidTagsFileName);
            VoidTags = JsonSerializer.Deserialize<string[]>(voidTagsJson)?
                                    .Select(t => t.ToLower()) // המרה לאותיות קטנות לחיפוש עקבי
                                    .ToArray() ?? Array.Empty<string>();

            Console.WriteLine($"HtmlHelper: נטענו בהצלחה {AllTags.Length} תגיות כלליות ו-{VoidTags.Length} תגיות ריקות.");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"שגיאה: קובץ JSON לא נמצא. וודא ש-{AllTagsFileName} ו-{VoidTagsFileName} נמצאים בנתיב ההרצה.");
            AllTags = Array.Empty<string>();
            VoidTags = Array.Empty<string>();
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"שגיאת JSON בטעינת הקבצים: {ex.Message}");
            AllTags = Array.Empty<string>();
            VoidTags = Array.Empty<string>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"שגיאה בלתי צפויה בטעינת קובץ: {ex.Message}");
            AllTags = Array.Empty<string>();
            VoidTags = Array.Empty<string>();
        }
    }
}