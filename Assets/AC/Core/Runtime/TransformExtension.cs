using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.Core
{
    public static class TransformExtension 
    {
        public static void SetLayerRecursively(this Transform trans, int layer)
        {
            trans.gameObject.layer = layer;
            foreach (Transform child in trans)
                child.gameObject.layer = layer;
        }
    }
}

