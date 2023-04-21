using System.Globalization;

namespace ForgottenAdventuresTokenOrganizer.Usefull
{
    public static class Extensions
    {
        public static string ToTitleCase(this string title)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title.ToLower());
        }
    }
}
