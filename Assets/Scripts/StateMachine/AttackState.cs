using Scripts.Controller;
using UnityEngine;

public class AttackState : IUnitState
{
    private readonly UnitController _unit;
    private float _attackCooldown;
    private const float PercentOfSize = 1.05f;

    public AttackState(UnitController unit) => _unit = unit;

    public void Enter() => _attackCooldown = 0f;

    public void Tick(float deltaTime)
    {
        if (_unit.Target == null || !_unit.Target.IsAlive)
        {
            _unit.Target = null;
            _unit.ChangeState(new IdleState(_unit));
            return;
        }

        float dist = Vector3.Distance(_unit.transform.position,
                                      _unit.Target.transform.position);
        var minDist = PercentOfSize *(_unit.AvrRadius + _unit.Target.AvrRadius);
        if (dist > minDist)
        {
            _unit.ChangeState(new MoveToTargetState(_unit));
            return;
        }

        _attackCooldown -= deltaTime;
        if (_attackCooldown <= 0f)
        {
            _attackCooldown = _unit.Stats.AtkSpd;
            if (!_unit.CheckForUnitTransfer())
            {
                _unit.Target.TakeDamage(_unit.Stats.ATK, _unit);
            }
            else
            {
                var transferUnit = _unit.Target;
                _unit.Target = null;
                _unit.ChangeState(new IdleState(_unit));
                GameEvents.RaiseUnitTransfer(transferUnit);
            }
        }
    }

    public void Exit() { }
}
