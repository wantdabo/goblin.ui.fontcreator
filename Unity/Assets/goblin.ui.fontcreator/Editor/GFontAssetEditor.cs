using GoblinFramework.UI.FontCreator;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Text;
using UnityEditor;
using UnityEditor.U2D;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.U2D;

namespace GoblinFramework.UI.FontCreator
{
    [CustomEditor(typeof(GFontAsset))]
    public class GFontAssetEditor : Editor
    {
        private ReorderableList reorderableList;

        private SerializedProperty spritesProperty;

        private void OnEnable()
        {
            if (null == serializedObject) return;

            spritesProperty = serializedObject.FindProperty("sprites");
            reorderableList = new ReorderableList(serializedObject, spritesProperty, true, true, true, true);

            reorderableList.drawHeaderCallback = (Rect rect) => GUI.Label(rect, "GSprites");
            reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty item = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, item);
            };
        }

        public override void OnInspectorGUI()
        {
            if (null == reorderableList) return;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("FontName:", GUILayout.Width(65));
            var fontNameProperty = serializedObject.FindProperty("fontName");
            fontNameProperty.stringValue = EditorGUILayout.TextField(fontNameProperty.stringValue);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Pading:", GUILayout.Width(65));
            var paddingProperty = serializedObject.FindProperty("padding");
            paddingProperty.intValue = EditorGUILayout.IntField(paddingProperty.intValue);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Tracking:", GUILayout.Width(65));
            var trackingProperty = serializedObject.FindProperty("tracking");
            float newTracking = EditorGUILayout.FloatField(trackingProperty.floatValue);
            trackingProperty.floatValue = newTracking;
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
            EditorGUILayout.Space(10);

            reorderableList.DoLayoutList();

            if (GUILayout.Button("Generate Font")) GenerateFont();
        }

        private void GenerateFont()
        {
            #region Make Texture
            GSprite[] gSprites = new GSprite[spritesProperty.arraySize];
            for (int i = 0; i < gSprites.Length; i++)
            {
                var property = spritesProperty.GetArrayElementAtIndex(i);
                var indexProperty = property.FindPropertyRelative("index");
                var spriteProperty = property.FindPropertyRelative("texture");
                gSprites[i] = new GSprite()
                {
                    index = indexProperty.stringValue,
                    texture = spriteProperty.objectReferenceValue as Texture2D,
                };
            }

            float padding = serializedObject.FindProperty("padding").intValue;
            float width = 0;
            float maxHeight = int.MinValue;
            foreach (var sprite in gSprites)
            {
                width += sprite.texture.width + padding;
                maxHeight = Mathf.Max(maxHeight, sprite.texture.height);
            }
            Texture2D texture = new Texture2D((int)width, (int)maxHeight + 10, TextureFormat.RGBA32, false);
            texture.name = "TEMP_FONT_TEXTURE";

            Color32 alphaColor = Color.white;
            alphaColor.a = 0x0;
            for (int i = 0; i < texture.width; i++)
            {
                for (int j = 0; j < texture.height; j++)
                {
                    texture.SetPixel(i, j, alphaColor);
                }
            }
            float offset = 0;
            List<Vector4> UV_XY_WH_LIST = new List<Vector4>();
            List<Vector2> VERT_WH_LIST = new List<Vector2>();
            foreach (var gSprite in gSprites)
            {
                var tex = gSprite.texture;
                for (int i = 0; i < tex.width; i++)
                {
                    for (int j = 0; j < tex.height; j++)
                    {
                        var color = tex.GetPixel(i, j);
                        texture.SetPixel((int)offset + i, j, color);
                    }
                }
                UV_XY_WH_LIST.Add(new Vector4()
                {
                    // UV X
                    x = offset / texture.width,
                    // UV Y
                    y = 0,

                    // UV W
                    z = tex.width / (float)texture.width,
                    // UV H
                    w = tex.height / (float)texture.height,
                });
                VERT_WH_LIST.Add(new Vector2(tex.width, -tex.height));
                offset += tex.width + padding;
            }
            texture.Apply();

            string fontName = serializedObject.FindProperty("fontName").stringValue;
            var assetPath = AssetDatabase.GetAssetPath(serializedObject.targetObject);
            FileInfo fileInfo = new FileInfo(Application.dataPath.Replace("Assets", "") + assetPath);

            var path = assetPath.Replace($"{fileInfo.Name}", "");
            #endregion

            #region Make Material
            Material mat = new Material(Shader.Find("UI/Unlit/Transparent"));
            mat.SetTexture("_MainTex", texture);
            #endregion

            #region Make Font
            var tracking = serializedObject.FindProperty("tracking").floatValue;
            var font = new Font();
            font.name = fontName;
            font.material = mat;
            CharacterInfo[] characterInfos = new CharacterInfo[gSprites.Length];
            for (int i = 0; i < characterInfos.Length; i++)
            {
                var UV_XY_WH = UV_XY_WH_LIST[i];
                var VERT_WH = VERT_WH_LIST[i];
                characterInfos[i] = new CharacterInfo()
                {
                    index = gSprites[i].index.ToCharArray()[0],
                    uv = new Rect(new Vector2(UV_XY_WH.x, UV_XY_WH.y), new Vector2(UV_XY_WH.z, UV_XY_WH.w)),
                    vert = new Rect(Vector2.zero, VERT_WH),
                    advance = (int)(VERT_WH.x * tracking),
                };
            }
            font.characterInfo = characterInfos;
            var fontSavePath = $"{path}{fontName}.fontsettings";
            if (false == string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(fontSavePath))) AssetDatabase.DeleteAsset(fontSavePath);

            AssetDatabase.CreateAsset(font, fontSavePath);
            AssetDatabase.AddObjectToAsset(mat, fontSavePath);
            AssetDatabase.AddObjectToAsset(texture, fontSavePath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            #endregion
        }
    }
}
