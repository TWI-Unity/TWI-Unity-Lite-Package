namespace TWILite.Utilities.MenuItemAutogen
{
    using System;
    using System.CodeDom.Compiler;
    using System.IO;
    using System.Linq;
    using TWILite.Extensions;

    [Serializable]
    public sealed class MenuItemAutogenItem_OpenWith : MenuItemAutogenItem
    {
        public const string NAMEOF_APP_PATH = nameof(AppPath);
        public const string NAMEOF_EXTENSIONS = nameof(Extensions);
        public const string NAMEOF_ARGUMENTS = nameof(Arguments);

        public override bool HasMethodValidation => true;
        public override bool UseMethodLambdaOperator => true;
        public override bool UseMethodValidationLambdaOperator => true;

        public string AppPath;
        public string[] Extensions;
        public string[] Arguments;

        public MenuItemAutogenItem_OpenWith() { }
        public MenuItemAutogenItem_OpenWith(string menuPath, string methodName, string appPath = null, string[] arguments = null, params string[] extensions) : base(menuPath, methodName)
        {
            AppPath = appPath;
            Extensions = extensions;
            Arguments = arguments;
        }

        public override void CompileMethod(IndentedTextWriter writer) => writer.WriteLine($"Open(Selection.objects, @\"{(AppPath ?? string.Empty)}\", @\"{string.Join("\", @\"", Arguments ?? new string[0])}\");");
        public override void CompileMethodValidation(IndentedTextWriter writer) => writer.WriteLine($"ValidateExtensions(Selection.objects, \"{string.Join("\", \"", Extensions ?? new string[0])}\");");

        public override string Validate()
        {
            if (base.Validate() is string warning) return warning;
            else if (string.IsNullOrWhiteSpace(AppPath)) return "The property 'AppPath' was not specified.";
            else if (AppPath.Any(Path.GetInvalidPathChars().Contains)) return "The property 'AppPath' contains invalid characters.";
            else if (Extensions == null || Extensions.Length == 0) return "The property 'Extensions' contains no elements.";
            else if (Extensions.Any(s => s.Any(Path.GetInvalidFileNameChars().Contains))) return "The property 'Extensions' contains one or more invalid elements";
            else if (Arguments != null && Arguments.Any(s => s.Any(Path.GetInvalidPathChars().Contains))) return "The property 'Arguments' contains one or more invalid elements";
            else return null;
        }
    }
}