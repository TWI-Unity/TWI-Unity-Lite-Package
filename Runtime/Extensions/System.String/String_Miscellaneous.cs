namespace TWILite.Extensions
{
    public static class String_Miscellaneous
    {
        public static string SubSuffix(this string str, int length, string suffix) => str != null && str.Length > length ? str.Substring(0, length) + suffix : str;
    }
}