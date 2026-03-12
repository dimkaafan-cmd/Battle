using Scripts.Data;
using UnityEngine;

namespace Scripts.Controller{
    public class ArmySpawner : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private GameConfig  gametConfig;

        [Header("Layout")]
        [SerializeField] private int   unitsPerArmy = 20;
        [SerializeField] private int   columns      = 3;

        public ArmyModel SpawnArmy(Transform parent, ArmyType type, Bounds bounds, bool facingRight)
        {
            var army = new ArmyModel(type);
            
            var unitMaxSize = 0f;   
            for (int i = 0; i < unitsPerArmy; i++)
            {
                var shape    = gametConfig.shapes[Random.Range(0, gametConfig.shapes.Length)];
                var size     = gametConfig.sizes[Random.Range(0,  gametConfig.sizes.Length)];
                var color    = gametConfig.colors[Random.Range(0, gametConfig.colors.Length)];

                var stats    = UnitFactory.CreateStats(gametConfig.unitConfig, shape, size, color);
                var identity = UnitFactory.CreateIdentity(shape, size, color);

                var go = Instantiate(shape.prefab, bounds.center, Quaternion.identity, parent);
                go.transform.localScale = size.scale;
                go.transform.rotation   = Quaternion.Euler(0f, facingRight ? 0f : 180f, 0f);
                go.name = $"{type}_{identity}_{i}";

                var controller = go.GetComponent<UnitController>();
                controller.Initialize(stats, identity, i, gametConfig);
                unitMaxSize = Mathf.Max(unitMaxSize, controller.AvrRadius);

                army.AddUnit(controller);
            }

            var rows = unitsPerArmy / columns + 1;
            var dx = Mathf.Max(bounds.size.x / columns, unitMaxSize * 2.5f);
            var dy = Mathf.Max(bounds.size.y / rows, unitMaxSize * 2.5f);

            for(var i = 0; i <  army.Units.Count; i++)
            {
                var randDelta = new Vector3(Random.Range(-dx, dx), Random.Range(-dy, dy), 0);
                var deltaPos = new Vector3(dx * (i % columns) - bounds.size.x/2
                , dy * (i / columns) - bounds.size.y/2, 0) + randDelta * 0;

                army.Units[i].transform.position =  bounds.center + deltaPos;
            }

            return army;
        }
    }
}
