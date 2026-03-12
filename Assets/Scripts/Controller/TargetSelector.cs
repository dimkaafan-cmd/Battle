using UnityEngine;

namespace Scripts.Controller
{
    public static class TargetSelector
    {
        public static UnitController GetNearest(UnitController self, ArmyModel enemies)
        {
            if (enemies == null || enemies.IsDefeated) return null;

            UnitController nearest = null;
            float          minDist  = float.MaxValue;

            foreach (var enemy in enemies.Units)
            {
                if (!enemy.IsAlive) continue;
                float d = Vector3.Distance(self.transform.position, enemy.transform.position);
                if (d < minDist) { minDist = d; nearest = enemy; }
            }

            return nearest;
        }
    }
}
