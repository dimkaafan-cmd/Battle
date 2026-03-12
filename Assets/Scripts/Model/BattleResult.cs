public class BattleResult
{
    public ArmyType Winner { get; }
    public int SurvivorsCount { get; }

    public BattleResult(ArmyType winner, int survivors)
    {
        Winner = winner;
        SurvivorsCount = survivors;
    }
}
