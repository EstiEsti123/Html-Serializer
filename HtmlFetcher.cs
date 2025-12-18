using System;
using System.Net.Http;
using System.Threading.Tasks;

public class HtmlFetcher
{
    /// <summary>
    /// מבצע קריאת GET א-סינכרונית לכתובת URL ומחזיר את תוכן ה-HTML.
    /// </summary>
    public async Task<string> Load(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string html = await response.Content.ReadAsStringAsync();
                return html;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"שגיאה במהלך קריאת ה-URL: {e.Message}");
                return string.Empty;
            }
        }
    }
}