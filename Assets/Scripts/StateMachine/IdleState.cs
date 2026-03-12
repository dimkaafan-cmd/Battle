using Scripts.Controller;

public class IdleState : IUnitState
{
    private readonly UnitController _unit;
    private float _searchTimer;
    private const float SearchInterval = 0.3f;

    public IdleState(UnitController unit) => _unit = unit;

    public void Enter() => _searchTimer = 0f;

    public void Tick(float deltaTime)
    {
        _searchTimer -= deltaTime;
        if (_searchTimer > 0f) 
        {
            return;
        }
        _searchTimer = SearchInterval;

        var target = TargetSelector.GetNearest(_unit, _unit.EnemyArmy);
        if (target != null)
        {
            _unit.Target = target;
            _unit.ChangeState(new MoveToTargetState(_unit));
        }
    }

    public void Exit() { }
}
