using Scripts.Controller;
using UnityEngine;

public class MoveByDirState : IUnitState
{
    private const float MoveInterval = 0.3f;
    private readonly UnitController _unit;
    
    private Vector3 _dir = Vector3.zero;
    private float _moveTimer;

    public MoveByDirState(UnitController unit, Vector3 dir){
        _unit = unit;
        _dir = dir;
    }

    public void Enter() { _moveTimer = MoveInterval;}

    public void Tick(float deltaTime)
    {
        _moveTimer -= deltaTime;
        if (_moveTimer < 0 || !MoveChecker.CanMove(_unit, _dir))
        {
            _unit.Target = null;
            _unit.ChangeState(new IdleState(_unit));
            return;
        }

        _unit.transform.position += _dir * _unit.Stats.Speed * deltaTime;

        if (_dir != Vector3.zero)
            _unit.transform.rotation = Quaternion.LookRotation(_dir);
    }

    public void Exit() { }
}
