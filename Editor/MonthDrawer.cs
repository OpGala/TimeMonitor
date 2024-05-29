using TimeMonitor.Data;
using UnityEditor;
using UnityEngine;

namespace TimeMonitor.Editor
{
      [CustomPropertyDrawer(typeof(Month))]
      public class MonthDrawer : PropertyDrawer
      {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                  EditorGUI.BeginProperty(position, label, property);

                  SerializedProperty monthNumberProperty = property.FindPropertyRelative("monthNumber");
                  SerializedProperty daysProperty = property.FindPropertyRelative("days");

                  label = new GUIContent("Month " + monthNumberProperty.intValue);
                  position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

                  EditorGUI.indentLevel++;
                  EditorGUI.PropertyField(position, daysProperty, true);
                  EditorGUI.indentLevel--;

                  EditorGUI.EndProperty();
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                  return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("days"), label, true);
            }
      }
}