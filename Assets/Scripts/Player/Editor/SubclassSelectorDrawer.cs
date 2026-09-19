#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AwCon
{
    // 已修复编码乱码的注释。
    [CustomPropertyDrawer(typeof(SubclassSelectorAttribute))]
    public class SubclassSelectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 已修复编码乱码的注释。
            if (property.propertyType != SerializedPropertyType.ManagedReference)
            {
                EditorGUI.HelpBox(position, "说明", MessageType.Error);
                return;
            }

            // 已修复编码乱码的注释。
            position = EditorGUI.PrefixLabel(position, label);

            // 已修复编码乱码的注释。
            string currentTypeName = "说明";
            if (!string.IsNullOrEmpty(property.managedReferenceFullTypename))
            {
                // 已修复编码乱码的注释。
                currentTypeName = property.managedReferenceFullTypename.Split(' ').Last().Split('.').Last();
            }

            // 已修复编码乱码的注释。
            if (EditorGUI.DropdownButton(position, new GUIContent(currentTypeName), FocusType.Keyboard))
            {
                ShowDropdownMenu(property);
            }
        }

        private void ShowDropdownMenu(SerializedProperty property)
        {
            GenericMenu menu = new GenericMenu();

            // 已修复编码乱码的注释。
            menu.AddItem(new GUIContent("说明"), string.IsNullOrEmpty(property.managedReferenceFullTypename), () =>
            {
                ApplyInstance(property, null);
            });
            menu.AddSeparator("");

            Type baseType = fieldInfo.FieldType;
            var derivedTypes = TypeCache.GetTypesDerivedFrom(baseType)
                .Where(t => !t.IsAbstract && !t.IsInterface && !t.IsGenericType); // 已修复编码乱码的注释。

            foreach (Type type in derivedTypes)
            {
                string menuPath = type.Name; // 已修复编码乱码的注释。

                // 已修复编码乱码的注释。
                bool isSelected = property.managedReferenceFullTypename.EndsWith(type.Name);

                menu.AddItem(new GUIContent(menuPath), isSelected, () =>
                {
                    // 已修复编码乱码的注释。
                    object newInstance = Activator.CreateInstance(type);
                    ApplyInstance(property, newInstance);
                });
            }

            menu.ShowAsContext();
        }

        // 已修复编码乱码的注释。
        private void ApplyInstance(SerializedProperty property, object instance)
        {
            property.serializedObject.Update();
            property.managedReferenceValue = instance;
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
