namespace TWILiteEditor.Utilities.MenuItemAutogen
{
    using TWILite.Utilities.MenuItemAutogen;
    using UnityEditor;

    [CustomEditor(typeof(MenuItemAutogenCollection_ChangeEXT))]
    public sealed class MenuItemAutogenEditor_ChangeEXT : MenuItemAutogenEditor<MenuItemAutogenCollection_ChangeEXT, MenuItemAutogenItem_ChangeEXT>
    {
        protected override string OnGetPropertyDisplayName(SerializedProperty property) => "Extension";
    }
}