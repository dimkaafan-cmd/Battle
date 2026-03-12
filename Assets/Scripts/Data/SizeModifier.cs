using UnityEngine;

namespace Scripts.Data
{
    public enum UnitSize { Small, Big }

    [CreateAssetMenu(menuName = "BattleClash/Size Modifier")]
    public class SizeModifier : ScriptableObject
    {
        public UnitSize size;
        public Vector3  scale = Vector3.one;
        public int      hpDelta;
    }
}
