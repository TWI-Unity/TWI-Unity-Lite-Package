namespace TWILite.Utilities.MenuItemAutogen
{
    using System;
    using System.CodeDom.Compiler;

    [Serializable]
    public sealed class MenuItemAutogenItem_ChangeEXT : MenuItemAutogenItem
    {
        public const string NAMEOF_TARGET = nameof(Target);
        public const string NAMEOF_SUPPORTED = nameof(Supported);

        public override bool HasMethodValidation => true;
        public override bool UseMethodLambdaOperator => true;
        public override bool UseMethodValidationLambdaOperator => true;

        public string Target;
        public string[] Supported;

        public MenuItemAutogenItem_ChangeEXT() { }
        public MenuItemAutogenItem_ChangeEXT(string menuPath, string methodName, string target = null, params string[] supported) : base(menuPath, methodName)
        {
            this.Target = target;
            this.Supported = supported;
        }

        public override void CompileMethod(IndentedTextWriter writer) => writer.WriteLine($"ChangeExtensions(Selection.objects, \"{Target}\", LoadOptions.Select);");
        public override void CompileMethodValidation(IndentedTextWriter writer) => writer.WriteLine($"ValidateExtensions(Selection.objects, \"{Target}\", \"{string.Join("\", \"", Supported ?? new string[0])}\");");
    }
}