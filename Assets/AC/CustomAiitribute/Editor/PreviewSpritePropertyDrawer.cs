using AC.Attribute;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AC.Attribute
{
    [CustomPropertyDrawer(typeof(PreviewSpriteAttribute))]
    public class PreviewSpritePropertyDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var fieddata = this.attribute as PreviewSpriteAttribute;
            if (EditorGUIUtility.HasObjectThumbnail(fieldInfo.FieldType))
            {
                return fieddata.SizeImage.y;
            }
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(EditorGUIUtility.HasObjectThumbnail(fieldInfo.FieldType))
            {
                var fieddata = this.attribute as PreviewSpriteAttribute;
                EditorGUI.BeginProperty(position, label, property);
                int indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                //Draw Lable
                EditorGUI.LabelField(position, label);

                //Draw Sprite
                position.x += EditorGUIUtility.labelWidth;
                position.width = fieddata.SizeImage.x;
                position.height = fieddata.SizeImage.y;
                EditorGUI.ObjectField(position, property, fieldInfo.FieldType, new GUIContent());

                EditorGUI.indentLevel = indent;
                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
}

