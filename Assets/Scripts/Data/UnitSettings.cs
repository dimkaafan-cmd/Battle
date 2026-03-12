using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Data
{
    [CreateAssetMenu(fileName = "UnitSettings", menuName = "BattleClash/UnitSettings")]
    public class UnitSettings : ScriptableObject
    {
        [Serializable]
        public struct ColorItem
        {
            public UnitColor unitColor;
            public Material material; 
            public Color color;
        }

        [Serializable]
        public struct SizeItem
        {
            public UnitSize unitSize;
            public float scale; 
        }

        [Serializable]
        public struct ShapeItem
        {
            public UnitShape unitShape;
            public Mesh mesh; 
        }
        [SerializeField] public List<ColorItem> colors; 
        [SerializeField] public List<SizeItem> sizes;
        [SerializeField] public List<ShapeItem> shapes; 
    }
}
