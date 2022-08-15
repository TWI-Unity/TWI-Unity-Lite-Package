namespace TWILite.Utilities.MenuItemAutogen
{
    public sealed class MenuItemAutogenCollection_ChangeEXT : MenuItemAutogenCollection<MenuItemAutogenItem_ChangeEXT>
    {
        public override string GetDisplayName() => "Change EXT";
        public override string[] GetUsingNamespaces() => new string[]
        {
            "UnityEditor",
            "",
            "static TWILiteEditor.Utilities.AssetUtilities"
        };

        public override void Reset()
        {
            Reset("Change Extension");
            Add(new MenuItemAutogenItem_ChangeEXT("Text Asset/C# Script (.cs)", "CS", ".cs", ".lua", ".json", ".shader", ".txt", ".xml"));
            Add(new MenuItemAutogenItem_ChangeEXT("Text Asset/Lua Script (.lua)", "LUA", ".lua", ".cs", ".json", ".shader", ".txt", ".xml"));
            Add(new MenuItemAutogenItem_ChangeEXT("Text Asset/Json Document (.json)", "JSON", ".json", ".cs", ".lua", ".shader", ".txt", ".xml"));
            Add(new MenuItemAutogenItem_ChangeEXT("Text Asset/Shader Script (.shader)", "SHADER", ".shader", ".cs", ".lua", ".json", ".txt", ".xml"));
            Add(new MenuItemAutogenItem_ChangeEXT("Text Asset/Text Document (.txt)", "TXT", ".txt", ".cs", ".lua", ".json", ".shader", ".xml"));
            Add(new MenuItemAutogenItem_ChangeEXT("Text Asset/Xml Document (.xml)", "XML", ".xml", ".cs", ".lua", ".json", ".shader", ".txt"));
        }
    }
}