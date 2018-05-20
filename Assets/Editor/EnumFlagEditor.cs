using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace VoidWars {
    [CustomPropertyDrawer(typeof(EnumFlagAttribute))]
    public class EnumFlagDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EnumFlagAttribute flagSettings = (EnumFlagAttribute)attribute;
            Enum targetEnum = GetBaseProperty<Enum>(property);
            // Yuk. As if this code wasn't broken enough, when you increase the size of an editor array, and it hits
            // an item with an enum array in it, on the first draw it hasn't created the new item so this code throws
            // an exception. Next time through seems fine.
            if (targetEnum == null)
                return;

            string propName = flagSettings.enumName;
            if (string.IsNullOrEmpty(propName))
                propName = property.name;

            EditorGUI.BeginProperty(position, label, property);
            Enum enumNew = EditorGUI.EnumFlagsField(position, propName, targetEnum);
            property.intValue = (int)Convert.ChangeType(enumNew, targetEnum.GetType());
            EditorGUI.EndProperty();
        }

        static T GetBaseProperty<T>(SerializedProperty prop) {
            try {
                return GetBasePropertyInner<T>(prop);
            }
            catch {
                return default(T);
            }
        }

        static T GetBasePropertyInner<T>(SerializedProperty prop) {
            // Separate the steps it takes to get to this property
            string[] separatedPaths = prop.propertyPath.Split('.');

            // Go down to the root of this serialized property
            System.Object reflectionTarget = prop.serializedObject.targetObject as object;
            // Walk down the path to get the target object
            for(int i = 0; i < separatedPaths.Length; ++i) {
                var path = separatedPaths[i];
                if (path == "Array") {
                    ++i;
                    var array = (Array)reflectionTarget;
                    var leftIndex = separatedPaths[i].IndexOf('[');
                    var rightIndex = separatedPaths[i].IndexOf(']');
                    var indexStr = separatedPaths[i].Substring(leftIndex + 1, rightIndex - leftIndex - 1);
                    var index = int.Parse(indexStr);
                    reflectionTarget = array.GetValue(index);
                }
                else 
                {
                    FieldInfo fieldInfo = reflectionTarget.GetType().GetField(path);
                    reflectionTarget = fieldInfo.GetValue(reflectionTarget);
                }
            }
            return (T)reflectionTarget;
        }
    }
}