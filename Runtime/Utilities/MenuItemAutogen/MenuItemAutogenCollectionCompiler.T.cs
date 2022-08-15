namespace TWILite.Utilities.MenuItemAutogen
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using TWILite.Extensions;
    using UnityEngine;

    public sealed class MenuItemAutogenCollectionCompiler<T> where T : MenuItemAutogenItem
    {
        private const string LAMBDA_OPERATOR = " => ";

        private const string UNICODE_AMPERSAND = "\\u0026";
        private const string UNICODE_FRACTION_SLASH = "\\u2044";
        private const string UNICODE_FULL_WIDTH_AMPERSAND = "\\uFF06";
        private const string UNICODE_NUMBER_SIGN = "\\u0023";
        private const string UNICODE_LOW_LINE = "\\u005F";
        private const string UNICODE_PERCENT_SIGN = "\\u0025";
        private const string UNICODE_QOUTATION_MARK = "\\u0022";
        private const string UNICODE_REVERSE_SOLIDUS = "\\u005C";
        private const string UNICODE_SOLIDUS = "\\u002F";
        private const string UNICODE_ZERO_WIDTH_SPACE = "\\u200B";

        public readonly MenuItemAutogenCollection<T> collection;
        public event Action<string> OnWarning;

        public bool IsRunning { get; private set; }

        public int Index { get; private set; }
        public int Count => collection.Count;
        public float Progress { get; private set; }

        public MenuItemAutogenCollectionCompiler(MenuItemAutogenCollection<T> collection) => this.collection = collection;

        public IEnumerator Start(IndentedTextWriter writer)
        {
            IsRunning = true;
            string warning = null;
            string @class = collection.ClassName;
            if (string.IsNullOrWhiteSpace(@class)) warning = "The property 'ClassName' was not specified.";
            else if (!@class.StartsWithLetter('_')) warning = "The property 'ClassName' must start with a letter or underscore.";
            else if (!@class.IsAlphaNumeric('_')) warning = "The property 'ClassName' must only contain alpha-numeric or underscore characters.";
            if (warning != null && OnWarning != null) OnWarning(warning + $" Using fallback: '{@class = collection.GetClassNameFallback()}'");

            warning = null;
            string @namespace = collection.Namespace;
            if (string.IsNullOrWhiteSpace(@namespace)) @namespace = null;
            else
            {
                string subName;
                string[] subNames = @namespace.Split('.');
                for (int i = 0; i < subNames.Length; i++)
                {
                    subName = subNames[i];
                    if (string.IsNullOrWhiteSpace(subName)) warning = $"The property 'Namespace' sub-name at index '{i}' was not specified.";
                    else if (!subName.StartsWithLetter('_')) warning = $"The property 'Namespace' sub-name at index '{i}' must start with a letter or underscore.";
                    else if (!subName.IsAlphaNumeric('_')) warning = $"The property 'Namespace' sub-name at index '{i}' must only contain alpha-numeric or underscore characters.";
                    if (warning != null && OnWarning != null)
                    {
                        OnWarning(warning + $" Skipping namespace");
                        @namespace = null;
                    }
                }
            }

            string @base = collection.RootMenuPath;
            if (string.IsNullOrWhiteSpace(@base)) @base = string.Empty;
            else @base = @base.Replace("\"", "'\"'") + '/';

            if (@namespace != null)
            {
                writer.WriteLine($"namespace {@namespace}");
                writer.WriteLine('{');
                writer.Indent++;
            }

            string[] usings = collection.GetUsingNamespaces();
            foreach (var @using in usings)
                if (string.IsNullOrWhiteSpace(@using)) writer.WriteLine();
                else writer.WriteLine("using " + @using + ';');
            
            writer.WriteLine();
            writer.WriteLine($"public static class {@class}");
            writer.WriteLine('{');
            writer.Indent++;

            string methodAttribute = "[MenuItem(\"Assets/{0}\")]";
            string methodFormat = "public static void {0}()";
            string validateAttribute = "[MenuItem(\"Assets/{0}\", true)]";
            string validateFormat = "public static bool {0}_Validate()";
            HashSet<string> methods = new HashSet<string>();
            HashSet<string> menus = new HashSet<string>();
            string methodName = null;
            string menuPath = null;
            T item;

            for (int i = 0; i < collection.Count; i++)
            {
                item = collection[i];
                warning = item.Validate();

                if (warning == null)
                    if (menus.Contains(menuPath = item.MenuPath)) warning = "Duplicate 'MenuPath' detected.";
                    else if (methods.Contains(methodName = item.MethodName)) warning = "Duplicate 'MethodName' detected.";

                if (warning != null) OnWarning?.Invoke(warning + $" Ignoring item at index '{i}' of collection.");
                else
                {
                    menus.Add(menuPath);
                    methods.Add(methodName);
                    menuPath = menuPath.Replace("\\", UNICODE_REVERSE_SOLIDUS);
                    //menuPath = menuPath.Replace("&", UNICODE_FULL_WIDTH_AMPERSAND);
                    menuPath = menuPath.Replace(" %", ' ' + UNICODE_ZERO_WIDTH_SPACE + UNICODE_PERCENT_SIGN);
                    menuPath = menuPath.Replace(" #", ' ' + UNICODE_ZERO_WIDTH_SPACE + UNICODE_NUMBER_SIGN);
                    menuPath = menuPath.Replace(" _", ' ' + UNICODE_ZERO_WIDTH_SPACE + UNICODE_LOW_LINE);
                    menuPath = menuPath.Replace(" &", ' ' + UNICODE_FULL_WIDTH_AMPERSAND);
                    menuPath = menuPath.Replace(" /%", ' ' + UNICODE_PERCENT_SIGN);
                    menuPath = menuPath.Replace(" /#", ' ' + UNICODE_NUMBER_SIGN);
                    menuPath = menuPath.Replace(" /&", ' ' + UNICODE_AMPERSAND);
                    menuPath = menuPath.Replace(" /_", ' ' + UNICODE_LOW_LINE);
                    menuPath = menuPath.Replace("//", UNICODE_FRACTION_SLASH);
                    menuPath = menuPath.Replace("\"", UNICODE_QOUTATION_MARK);
                    menuPath = @base + menuPath;

                    WriteMethod(writer, methodAttribute, menuPath, methodFormat, methodName, item.UseMethodLambdaOperator, item.CompileMethod);
                    if (item.HasMethodValidation) WriteMethod(writer, validateAttribute, menuPath, validateFormat, methodName, item.UseMethodValidationLambdaOperator, item.CompileMethodValidation);
                }

                Index = i;
                Progress = (float)i / collection.Count;
                yield return null;
            }

            writer.Indent--;
            writer.WriteLine('}');
            if (@namespace != null)
            {
                writer.Indent--;
                writer.WriteLine('}');
            }

            IsRunning = false;
        }
        private void WriteMethod(IndentedTextWriter writer, string menuFormat, string menuPath, string methodFormat, string methodName, bool useLambdaExpression, Action<IndentedTextWriter> method)
        {
            writer.WriteLine(menuFormat, menuPath);
            writer.Write(methodFormat, methodName);

            if (useLambdaExpression)
            {
                writer.Write(LAMBDA_OPERATOR);
                method(writer);
            }
            else
            {
                writer.WriteLine();
                writer.WriteLine('{');
                writer.Indent++;
                method(writer);
                writer.Indent--;
                writer.WriteLine('}');
            }
            writer.WriteLine();
        }
    }
}