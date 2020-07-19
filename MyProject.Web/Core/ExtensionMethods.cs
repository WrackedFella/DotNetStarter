using Microsoft.AspNetCore.Http;
using System.Text;

namespace MyProject.Web.Core
{
    public static class ExtensionMethods
    {
        public static string GetUserName(this IHttpContextAccessor contextAccessor)
        {
            return contextAccessor?.HttpContext?.User?.Identity?.Name ?? "Test";
        }

        public static string AddSpaces(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                    newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }
    }
}