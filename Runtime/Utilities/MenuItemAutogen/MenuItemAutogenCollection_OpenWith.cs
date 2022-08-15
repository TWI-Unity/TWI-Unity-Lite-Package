namespace TWILite.Utilities.MenuItemAutogen
{
    public sealed class MenuItemAutogenCollection_OpenWith : MenuItemAutogenCollection<MenuItemAutogenItem_OpenWith>
    {
        public override string GetDisplayName() => "Open With";
        public override string[] GetUsingNamespaces() => new string[]
        {
            "TWILite.Extensions",
            "UnityEditor",
            "",
            "static TWILiteEditor.Utilities.AssetUtilities",
        };

        public override void Reset()
        {
            Reset("Open With");
            Add(new MenuItemAutogenItem_OpenWith("Text Asset/Atom", "Atom", @"%LOCALAPPDATA%\atom\atom.exe", null, ".cs", ".json", ".lua", ".shader", ".txt", ".xml"));
            Add(new MenuItemAutogenItem_OpenWith("Text Asset/Notepad++", "NotepadPP", @"%PROGRAMFILES%\Notepad++\notepad++.exe", null, ".cs", ".json", ".lua", ".shader", ".txt", ".xml"));
            Add(new MenuItemAutogenItem_OpenWith("Text Asset/VS Code", "VSCode", @"%PROGRAMFILES%\Microsoft VS Code\Code.exe", null, ".cs", ".json", ".lua", ".shader", ".txt", ".xml"));
            Add(new MenuItemAutogenItem_OpenWith("Texture Asset/GIMP 2.10", "GIMP2", @"%PROGRAMFILES%\GIMP 2\bin\gimp-2.10.exe", null, ".bmp", ".gif", ".ico", ".jpeg", ".jpg", ".png", ".tga", ".xcf"));
        }
    }
}