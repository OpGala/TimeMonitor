using TimeMonitor.Data;
using UnityEditor;
using UnityEngine;

namespace TimeMonitor.Editor
{
      [CustomPropertyDrawer(typeof(Day))]
      public class DayDrawer : PropertyDrawer
      {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                  EditorGUI.BeginProperty(position, label, property);

                  SerializedProperty dayNumberProperty = property.FindPropertyRelative("dayNumber");
                  SerializedProperty sessionsProperty = property.FindPropertyRelative("sessions");

                  label = new GUIContent("Day " + dayNumberProperty.intValue);
                  position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

                  EditorGUI.indentLevel++;
                  EditorGUI.PropertyField(position, sessionsProperty, true);
                  EditorGUI.indentLevel--;

                  EditorGUI.EndProperty();
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                  return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("sessions"), label, true);
            }

      }
}