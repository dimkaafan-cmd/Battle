using UnityEngine;

namespace Scripts.Data
{
    [CreateAssetMenu(menuName = "BattleClash/Mutation Config")]
    public class MutationConfig : ScriptableObject
    {
        [Header("Mutation Rules")]
        [Range(1, 10)]  public int   maxMutationStacks = 5;
        [Range(0f, 1f)] public float atkAbsorptionRate = 0.15f;
        [Range(0f, 1f)] public float hpAbsorptionRate  = 0.10f;

        [Header("Mutation Transfer")]
        [Range(1, 5)]  public int  minMutationLevelForTransfer = 5;
        [Range(1, 5)]  public int  minMutationDifLevelForTransfer = 3;
    }
}
