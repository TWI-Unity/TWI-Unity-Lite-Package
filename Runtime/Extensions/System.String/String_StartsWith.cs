using System.Linq;

namespace TWILite.Extensions
{
    public static class String_StartsWith
    {
        public static bool StartsWithDigit(this string str) => !string.IsNullOrEmpty(str) && char.IsDigit(str[0]);
        public static bool StartsWithDigit(this string str, string chars) => !string.IsNullOrEmpty(str) && (char.IsDigit(str[0]) || chars.Contains(str[0]));
        public static bool StartsWithDigit(this string str, params char[] chars) => !string.IsNullOrEmpty(str) && (char.IsDigit(str[0]) || chars.Contains(str[0]));

        public static bool StartsWithLetter(this string str) => !string.IsNullOrEmpty(str) && char.IsLetter(str[0]);
        public static bool StartsWithLetter(this string str, string chars) => !string.IsNullOrEmpty(str) && (char.IsLetter(str[0]) || chars.Contains(str[0]));
        public static bool StartsWithLetter(this string str, params char[] chars) => !string.IsNullOrEmpty(str) && (char.IsLetter(str[0]) || chars.Contains(str[0]));

        public static bool StartsWithLetterOrDigit(this string str) => !string.IsNullOrEmpty(str) && char.IsLetterOrDigit(str[0]);
        public static bool StartsWithLetterOrDigit(this string str, string chars) => !string.IsNullOrEmpty(str) && (char.IsLetterOrDigit(str[0]) || chars.Contains(str[0]));
        public static bool StartsWithLetterOrDigit(this string str, params char[] chars) => !string.IsNullOrEmpty(str) && (char.IsLetterOrDigit(str[0]) || chars.Contains(str[0]));
    }
}