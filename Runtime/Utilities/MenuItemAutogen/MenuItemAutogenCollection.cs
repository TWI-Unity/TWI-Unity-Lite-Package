namespace TWILite.Utilities.MenuItemAutogen
{
    using UnityEngine;

    public abstract class MenuItemAutogenCollection : ScriptableObject
    {
        public const string NAMEOF_CLASS_NAME = nameof(ClassName);
        public const string NAMEOF_LOCK_JSON = nameof(_lockJSON);
        public const string NAMEOF_NAMESPACE = nameof(Namespace);
        public const string NAMEOF_ROOT_MENU_PATH = nameof(RootMenuPath);

        [SerializeField] 
        protected string _lockJSON;

        [Tooltip("The class name of the C# script that will be compiled.")]
        public string ClassName;
        [Tooltip("The namespace of the C# script that will be compiled.")]
        public string Namespace;
        [Tooltip("This will be added in front of all menu paths of the items in the collection.")]
        public string RootMenuPath;

        public abstract int Count { get; }

        public MenuItemAutogenCollection() => _lockJSON = GetType().Name;

        public abstract void Clear();

        public string GetClassNameFallback()
        {
            string @class = GetType().Name.Trim('_');
            int index = @class.LastIndexOf('_');
            if (index > 0) @class = @class.Substring(index + 1);
            return @class + "_Autogen";
        }
        public string GetNamespaceFallback()
        {
            string @namespace = GetType().Namespace;
            if (string.IsNullOrWhiteSpace(@namespace)) @namespace = string.Empty;
            else if (@namespace.StartsWith("TWILite.")) @namespace = @namespace.Substring(@namespace.IndexOf('.'));
            return "TWIAutogen" + @namespace;
        }
        public abstract string GetDisplayName();
        public abstract string[] GetUsingNamespaces();

        public virtual void Reset() => Reset(null);
        protected void Reset(string rootMenuPath)
        {
            _lockJSON = GetType().Name;
            Namespace = GetNamespaceFallback();
            ClassName = GetClassNameFallback();
            RootMenuPath = rootMenuPath;
            Clear();
        }
    }
}