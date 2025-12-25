using System;
using System.IO;
using System.Text.Json;
using System.Linq;

public class HtmlHelper
{
    private const string AllTagsFileName = "HtmlTags.json";
    private const string VoidTagsFileName = "HtmlVoidTags.json";

    public string[] AllTags { get; private set; }

    public string[] VoidTags { get; private set; }

    public HtmlHelper()
    {
        LoadHtmlTags();
    }

    private void LoadHtmlTags()
    {
        try
        {
            string allTagsJson = File.ReadAllText(AllTagsFileName);
            AllTags = JsonSerializer.Deserialize<string[]>(allTagsJson)?
                                    .Select(t => t.ToLower()) // המרה לאותיות קטנות לחיפוש עקבי
                                    .ToArray() ?? Array.Empty<string>();
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
