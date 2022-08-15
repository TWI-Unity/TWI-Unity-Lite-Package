namespace TWILite.Utilities.MenuItemAutogen
{
    using System;
    using System.CodeDom.Compiler;
    using TWILite.Extensions;

    [Serializable]
    public abstract class MenuItemAutogenItem
    {
        public const string NAMEOF_MENU_PATH = nameof(MenuPath);
        public const string NAMEOF_METHOD_NAME = nameof(MethodName);

        public string MenuPath;
        public string MethodName;

        public virtual bool HasMethodValidation => false;

        public virtual bool UseMethodLambdaOperator => false;
        public virtual bool UseMethodValidationLambdaOperator => false;

        public MenuItemAutogenItem() { }
        public MenuItemAutogenItem(string menuPath, string methodName)
        {
            MenuPath = menuPath;
            MethodName = methodName;
        }

        public abstract void CompileMethod(IndentedTextWriter writer);
        public virtual void CompileMethodValidation(IndentedTextWriter writer) => throw new NotImplementedException();

        public virtual string Validate()
        {
            if (string.IsNullOrWhiteSpace(MenuPath)) return "The property 'MenuPath' was not specified.";
            else if (string.IsNullOrWhiteSpace(MethodName)) return "The property 'MethodName' was not specified.";
            else if (!MethodName.StartsWithLetter('_')) return "The property 'MethodName' must start with a letter or underscore.";
            else if (!MethodName.IsAlphaNumeric('_')) return "The property 'MethodName' may only contain letters, digits, or underscores.";
            else return null;
        }
    }
}