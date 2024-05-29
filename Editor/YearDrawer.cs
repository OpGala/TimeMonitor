using TimeMonitor.Data;
using UnityEditor;
using UnityEngine;

namespace TimeMonitor.Editor
{
      [CustomPropertyDrawer(typeof(Year))]
      public class YearDrawer : PropertyDrawer
      {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                  EditorGUI.BeginProperty(position, label, property);

                  SerializedProperty yearNumberProperty = property.FindPropertyRelative("yearNumber");
                  SerializedProperty monthsProperty = property.FindPropertyRelative("months");

                  label = new GUIContent("Year " + yearNumberProperty.intValue);
                  position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

                  EditorGUI.indentLevel++;
                  EditorGUI.PropertyField(position, monthsProperty, true);
                  EditorGUI.indentLevel--;

                  EditorGUI.EndProperty();
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                  return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("months"), label, true);
            }
      }
}