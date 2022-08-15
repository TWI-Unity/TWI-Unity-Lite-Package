namespace TWILite.Extensions
{
    using System.Linq;

    public static class String_AlphaNumeric
    {
        public static bool IsAlphaNumeric(this string str) => !string.IsNullOrEmpty(str) && str.Length > 0 && str.All(c => char.IsLetterOrDigit(c));
        public static bool IsAlphaNumeric(this string str, string chars) => !string.IsNullOrEmpty(str) && str.All(c => char.IsLetterOrDigit(c) || chars.Contains(c));
        public static bool IsAlphaNumeric(this string str, params char[] chars) => !string.IsNullOrEmpty(str) && str.All(c => char.IsLetterOrDigit(c) || chars.Contains(c));

        public static bool IsAlphaNumericWhitespace(this string str) => !string.IsNullOrEmpty(str) && str.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c));
        public static bool IsAlphaNumericWhitespace(this string str, string chars) => !string.IsNullOrEmpty(str) && str.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || chars.Contains(c));
        public static bool IsAlphaNumericWhitespace(this string str, params char[] chars) => !string.IsNullOrEmpty(str) && str.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || chars.Contains(c));

        public static string ToAlphaNumeric(this string str) => new string(str.Where(c => (char.IsLetterOrDigit(c))).ToArray());
        public static string ToAlphaNumeric(this string str, string chars) => new string(str.Where(c => (char.IsLetterOrDigit(c)) || chars.Contains(c)).ToArray());
        public static string ToAlphaNumeric(this string str, params char[] chars) => new string(str.Where(c => (char.IsLetterOrDigit(c)) || chars.Contains(c)).ToArray());

        public static string ToAlphaNumericWhitespace(this string str) => new string(str.Where(c => (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))).ToArray());
        public static string ToAlphaNumericWhitespace(this string str, string chars) => new string(str.Where(c => (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || chars.Contains(c))).ToArray());
        public static string ToAlphaNumericWhitespace(this string str, params char[] chars) => new string(str.Where(c => (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || chars.Contains(c))).ToArray());
    }
}