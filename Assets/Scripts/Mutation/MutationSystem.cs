using System.Collections.Generic;
using Scripts.Controller;
using Scripts.Data;

public class MutationSystem
{
    private readonly MutationConfig  _config;
    private readonly List<IMutation> _mutations;

    public MutationSystem(MutationConfig config)
    {
        _config    = config;
        _mutations = new List<IMutation>
        {
            new StatAbsorptionMutation(config)
            // Новые мутации регистрируются здесь — MutationSystem не трогаем
        };
    }

    public void OnUnitKilled(UnitController killer, UnitController victim)
    {
        if (killer == null || !killer.IsAlive) return;
        if (_config != null && killer.MutationStack.Count >= _config.maxMutationStacks) return;

        foreach (var mutation in _mutations)
            mutation.Apply(killer.Stats, victim.Stats);

        killer.MutationStack.Add(victim.Identity);
        killer.View.PlayMutationEffect(victim.Identity, killer.MutationStack.Count);
    }
}
