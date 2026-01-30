using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.Attribute
{
    public class PreviewSpriteAttribute : PropertyAttribute
    {
        public Vector2 SizeImage;
        public PreviewSpriteAttribute(float x, float y)
        {
            this.SizeImage = new Vector2(x, y);
        }
        public PreviewSpriteAttribute()
        {
            this.SizeImage = new Vector2(80, 80);
        }
    }
}

