using System.Text.RegularExpressions;

namespace ProiectStackOverflow.Helpers;

public class Summernote
{
    public static bool IsEditorEmpty(string html, int minLength = 0)
    {
        if (string.IsNullOrWhiteSpace(html))
            return true;

        // Elimină toate tag-urile HTML
        string textOnly = Regex.Replace(html, "<.*?>", string.Empty);
            
        // Elimină spațiile albe, newline-urile și tabs
        textOnly = textOnly.Replace("&nbsp;", " ");
        textOnly = textOnly.Trim();
            
        // Verifică dacă a mai rămas ceva text după curățare
        if (minLength > 0)
        {
            return string.IsNullOrWhiteSpace(textOnly) || textOnly.Length < minLength;
        }
            
        return string.IsNullOrWhiteSpace(textOnly);
    }

    public static string ReplaceRegex(string html)
    {
        // Elimină toate tag-urile HTML
        string textOnly = Regex.Replace(html, "<.*?>", string.Empty);
            
        // Elimină spațiile albe, newline-urile și tabs
        textOnly = textOnly.Replace("&nbsp;", " ");
        textOnly = textOnly.Trim();

        return textOnly;
    }
    
}