using Scripts.Data;
using UnityEngine;

/// <summary>
/// Уникальная фишка: убийца поглощает часть характеристик жертвы.
/// +15% от ATK жертвы, +10% от MaxHP жертвы (с восстановлением HP).
/// </summary>
public class StatAbsorptionMutation : IMutation
{
    private readonly MutationConfig _config;

    public StatAbsorptionMutation(MutationConfig config) => _config = config;

    public string MutationName => "Stat Absorption";

    public void Apply(UnitStats killer, UnitStats victim)
    {
        int atkBonus = Mathf.RoundToInt(victim.ATK   * _config.atkAbsorptionRate);
        int hpBonus  = Mathf.RoundToInt(victim.MaxHP * _config.hpAbsorptionRate);

        killer.ATK      += atkBonus;
        killer.MaxHP    += hpBonus;
        killer.CurrentHP = Mathf.Min(killer.CurrentHP + hpBonus, killer.MaxHP);
    }
}
