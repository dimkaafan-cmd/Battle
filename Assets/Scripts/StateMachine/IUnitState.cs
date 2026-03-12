public interface IUnitState
{
    void Enter();
    void Tick(float deltaTime);
    void Exit();
}
