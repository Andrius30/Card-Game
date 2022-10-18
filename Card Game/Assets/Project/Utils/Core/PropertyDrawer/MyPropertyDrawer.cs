using UnityEngine;
#if UNITY_ENGINE
using UnityEditor;
#endif
#if UNI
namespace Andrius.Core.Debuging
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class MyPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}
#endif
