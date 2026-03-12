using UnityEngine;

namespace Scripts.Data
{
    [CreateAssetMenu(menuName = "BattleClash/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Configs")]
        [SerializeField] public UnitConfig   unitConfig;
        [SerializeField] public ShapeModifier[] shapes;
        [SerializeField] public SizeModifier[]  sizes;
        [SerializeField] public ColorModifier[] colors;
        [SerializeField] public MutationConfig mutationConfig;
    }
}