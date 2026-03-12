using UnityEngine;

namespace Scripts.Data
{
    public enum UnitColor { Red, Green, Blue }

    [CreateAssetMenu(menuName = "BattleClash/Color Modifier")]
    public class ColorModifier : ScriptableObject
    {
        [Header("Visual")]
        public UnitColor unitColor;
        public Color     visualColor = Color.white;

        [Header("Stat Deltas")]
        public int   hpDelta;
        public int   atkDelta;
        public float speedDelta;
        public float atkspdDelta;
    }
}
