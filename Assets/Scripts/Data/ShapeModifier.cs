using UnityEngine;

namespace Scripts.Data
{
    public enum UnitShape { Cube, Sphere }

    [CreateAssetMenu(menuName = "BattleClash/Shape Modifier")]
    public class ShapeModifier : ScriptableObject
    {
        public UnitShape  shape;
        public GameObject prefab;
        public int        hpDelta;
        public int        atkDelta;
    }
}
