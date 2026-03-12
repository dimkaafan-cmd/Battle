using Scripts.Data;

namespace Scripts.Controller
{
    public static class TransferChecker
    {
        public static bool CheckForEnemyTransfer(UnitController unit, MutationConfig config)
        {
            if (unit == null || unit.Target == null || config == null)
            {
                return false;
            }
            return unit.MutationStack.Count >= config.minMutationLevelForTransfer &&
            (unit.MutationStack.Count - unit.Target.MutationStack.Count > config.minMutationDifLevelForTransfer);
        }
    }
}