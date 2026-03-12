using System.Linq;
using Scripts.Data;
using UnityEngine;

namespace Scripts.Controller
{
    public class ArmyController : MonoBehaviour
    {
        [SerializeField] private ArmyType type;

        [Header("Config")]
        [SerializeField] private GameConfig  gametConfig;

        public ArmyModel GetArmy()
        {
            var army = new ArmyModel(type);
            var units = gameObject.GetComponentsInChildren<UnitController>();
            var id = 0;
            foreach(var unit in units)
            {
                var unitShape = gametConfig.shapes.FirstOrDefault(it => it.shape == unit.Identity.shape);
                var unitSize = gametConfig.sizes.FirstOrDefault(it => it.size == unit.Identity.size);
                var unitColor = gametConfig.colors.FirstOrDefault(it => it.unitColor == unit.Identity.color);
                var stats    = UnitFactory.CreateStats(gametConfig.unitConfig, unitShape, unitSize, unitColor);

                unit.Initialize(stats, id++, gametConfig);
                unit.OwnArmy = army;

                army.AddUnit(unit);
            }
            return army;
        }

        public void CleanArmy()
        {
            var units = gameObject.GetComponentsInChildren<UnitController>();
            foreach(var unit in units)
            {
                Object.Destroy(unit.gameObject);
            }
        }
    }
}