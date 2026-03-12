using UnityEngine;

namespace Scripts.Data
{
    [CreateAssetMenu(menuName = "BattleClash/Unit Config")]
    public class UnitConfig : ScriptableObject
    {
        [Header("Base Stats")]
        public int   baseHP     = 100;
        public int   baseATK    = 10;
        public float baseSpeed  = 10f;
        public float baseAtkSpd = 1f;
    }
}