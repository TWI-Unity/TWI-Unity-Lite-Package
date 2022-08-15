namespace TWILite.Extensions
{
    using System;
    using System.Text;

    public static class String_Base64String
    {
        public static bool IsBase64String(this string str)
        {
            if (str == null) return false;
            else if (str.Length == 0) return true;
            else if (str.Length % 4 != 0) return false;

            int value = str.Length - str.LastIndexOf("=");
            if (value > 1 && value < str.Length || str[str.Length - 4] == '=') return false;
            else if (str[str.Length - 3] == '=') value = str.Length - 4;
            else if (str[str.Length - 2] == '=') value = str.Length - 3;
            else if (str[str.Length - 1] == '=') value = str.Length - 2;
            else value = str.Length - 1;

            for (int i = value; i > -1; i--)
            {
                value = (int)str[i];
                if ((value >= 65 && value <= 90) || (value >= 97 && value <= 122) || (value >= 48 && value <= 57) || value == 43 || value == 47) continue;
                else return false;
            }
            return true;
        }
    }
}