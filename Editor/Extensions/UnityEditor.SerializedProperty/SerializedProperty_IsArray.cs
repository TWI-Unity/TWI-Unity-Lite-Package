namespace TWILiteEditor.Extensions
{
    using UnityEditor;

    public static class SerializedProperty_IsArray
    {
        public static bool IsArray(this SerializedProperty property) => property.propertyType == SerializedPropertyType.Generic && property.isArray;
    }
}