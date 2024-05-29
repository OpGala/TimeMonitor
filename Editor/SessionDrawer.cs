using TimeMonitor.Data;
using UnityEditor;
using UnityEngine;

namespace TimeMonitor.Editor
{
      [CustomPropertyDrawer(typeof(Session))]
      public class SessionDrawer : PropertyDrawer
      {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                  EditorGUI.BeginProperty(position, label, property);

                  SerializedProperty startTimeProperty = property.FindPropertyRelative("startTime");
                  SerializedProperty endTimeProperty = property.FindPropertyRelative("endTime");

                  float halfWidth = position.width / 2;
                  var startRect = new Rect(position.x, position.y, halfWidth - 5, position.height);
                  var endRect = new Rect(position.x + halfWidth + 5, position.y, halfWidth - 5, position.height);

                  EditorGUI.PropertyField(startRect, startTimeProperty, GUIContent.none);
                  EditorGUI.PropertyField(endRect, endTimeProperty, GUIContent.none);

                  EditorGUI.EndProperty();
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                  return EditorGUIUtility.singleLineHeight;
            }
      }
}