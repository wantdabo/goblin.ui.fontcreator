﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoblinFramework.UI.FontCreator
{
    [Serializable]
    public struct GSprite
    {
        public string index;
        public Sprite sprite;
    }

    [CreateAssetMenu(fileName = "GFontAsset", menuName = "Create New GFont", order = 1)]
    [Serializable]
    public class GFontAsset : ScriptableObject
    {
        public string fontName = "GFont";
        public int padding = 4;

        [SerializeField]
        public List<GSprite> sprites;
    }
}