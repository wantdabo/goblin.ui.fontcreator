using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GoblinFramework.UI.FontCreator
{
    [CustomPropertyDrawer(typeof(GSprite))]
    public class GSpriteEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var indexRect = new Rect(position)
            {
                width = position.width * 0.1f,
            };

            var spriteRect = new Rect(indexRect)
            {
                x = indexRect.x + indexRect.width + 20,
                width = position.width * 0.9f - 20,
            };

            var indexProperty = property.FindPropertyRelative("index");
            indexProperty.stringValue = EditorGUI.TextField(indexRect, indexProperty.stringValue);
            if (indexProperty.stringValue.Length > 1) indexProperty.stringValue = indexProperty.stringValue.Substring(0, 1);

            var spriteProperty = property.FindPropertyRelative("texture");
            spriteProperty.objectReferenceValue = EditorGUI.ObjectField(spriteRect, spriteProperty.objectReferenceValue, typeof(Object), false);
        }
    }
}
