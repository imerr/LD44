#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class EnumFlagAttribute : PropertyAttribute {}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(EnumFlagAttribute))]
public class EnumFlagDrawer : PropertyDrawer{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumDisplayNames);
    }
}
#endif
