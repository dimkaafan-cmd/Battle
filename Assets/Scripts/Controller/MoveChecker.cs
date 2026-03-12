using UnityEngine;

namespace Scripts.Controller
{
    public static class MoveChecker
    {
        private const float PercentOfSize = 1.05f;
        public static bool CanMove(UnitController self, Vector3 movedDir)
        {
            foreach(var unit in self.OwnArmy.Units)
            {
                if (unit == null || unit == self)
                {
                    continue;
                }
                var dirToUnit = unit.transform.position - self.transform.position;
                var distBetwUnits = dirToUnit.magnitude;
                var minDist = PercentOfSize *(unit.AvrRadius + self.AvrRadius);
                dirToUnit.Normalize();
                if (distBetwUnits < minDist && Vector3.Dot(movedDir, dirToUnit) >= 0.3)
                {
                    return false;
                }
            }
            return true;
        }
    }
}