using Scripts.Controller;
using UnityEngine;

public class MoveToTargetState : IUnitState
{
    private readonly UnitController _unit;
    private const float PercentOfSize = 1.05f;

    public MoveToTargetState(UnitController unit) => _unit = unit;

    public void Enter() { }

    public void Tick(float deltaTime)
    {
        if (_unit.Target == null || !_unit.Target.IsAlive)
        {
            _unit.Target = null;
            _unit.ChangeState(new IdleState(_unit));
            return;
        }

        var movedDir = (_unit.Target.transform.position - _unit.transform.position).normalized;
        movedDir.Normalize();
        if (!MoveChecker.CanMove(_unit, movedDir))
        {
            _unit.Target = null;
            var normal = GetRandomNormalDir(movedDir);
            _unit.ChangeState(new MoveByDirState(_unit, normal));
            return;
        }

        var dist = Vector3.Distance(_unit.transform.position,
                                      _unit.Target.transform.position);
        var minDist = PercentOfSize *(_unit.AvrRadius + _unit.Target.AvrRadius);
        if (dist <= minDist)
        {
            _unit.ChangeState(new AttackState(_unit));
            return;
        }

        var dir = (_unit.Target.transform.position - _unit.transform.position).normalized;
        _unit.transform.position += dir * _unit.Stats.Speed * deltaTime;

        if (dir != Vector3.zero)
            _unit.transform.rotation = Quaternion.LookRotation(dir);
    }

    public void Exit() { }

    private Vector3 GetRandomNormalDir(Vector3 movedDir)
    {
        Vector3 normal;
        do{
            var rndDir = new Vector3{ x =  Random.Range(-1, 1), y = Random.Range(-1, 1), z = Random.Range(-1, 1)};
            rndDir.Normalize();
            normal = Vector3.Cross(movedDir, rndDir);
        } while(Vector3.Dot(movedDir, normal) != 0);

        normal.Normalize();
        return normal;
    }
}
