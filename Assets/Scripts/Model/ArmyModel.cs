using System;
using System.Collections.Generic;
using Scripts.Controller;

public enum ArmyType { TeamA, TeamB }

public class ArmyModel
{
    public ArmyType Type { get; }

    private readonly List<UnitController> _units = new();
    public IList<UnitController> Units => _units;
    public bool IsDefeated => _units.Count == 0;

    public event Action OnDefeated;

    public ArmyModel(ArmyType type) => Type = type;

    public void AddUnit(UnitController unit) 
    { 
        if (!_units.Contains(unit)) 
        {
            _units.Add(unit);
        }
    }

    public void RemoveUnit(UnitController unit)
    {
        if (_units.Remove(unit) && IsDefeated) {
            OnDefeated?.Invoke();
        }
    }

    public UnitController GetRandom()
    {
        return IsDefeated ? null : _units[UnityEngine.Random.Range(0, _units.Count)];
    }
}
