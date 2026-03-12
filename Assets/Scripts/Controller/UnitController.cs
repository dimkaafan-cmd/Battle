using System.Collections.Generic;
using System.Linq;
using Scripts.Data;
using UnityEngine;

namespace Scripts.Controller
{
    public class UnitController : MonoBehaviour
    {
        [SerializeField] private UnitIdentity _identity;
        [SerializeField] private UnitSettings unitSettings;
        [SerializeField, HideInInspector] private float maxSize;

        [SerializeField] private int id;
        public int Id {get => id; set => id = value;}

        public UnitStats      Stats     { get; private set; }
        public UnitIdentity   Identity  { get => _identity; private set => _identity = value; }
        public UnitView       View      { get; private set; }
        public ArmyModel      EnemyArmy { get; set; }
        public ArmyModel      OwnArmy { get; set; }
        public UnitController Target    { get; set; }

        public List<UnitIdentity> MutationStack { get; } = new();
        public bool IsAlive => Stats.IsAlive;
        public float AvrRadius => maxSize/2;

        private IUnitState _currentState;
        private bool _isDead;
        private GameConfig _gameConfig;

        public void Initialize(UnitStats stats, UnitIdentity identity, int unitId, GameConfig gameConfig)
        {
            _gameConfig = gameConfig;
            View = GetComponent<UnitView>();
            Stats     = stats;
            Identity  = identity;
            Id = unitId;
            ApplyIdentity.Apply(this, identity, unitSettings, out maxSize );
            View.Initialize(identity);
            ChangeState(new IdleState(this));
        }

        public void Initialize(UnitStats stats, int unitId, GameConfig gameConfig)
        {
            _gameConfig = gameConfig;
            View = GetComponent<UnitView>();
            Stats = stats;
            Id = unitId;
            View.Initialize(Identity);
            ChangeState(new IdleState(this));
        }

        private void Update()
        {
            if (!_isDead)
            {
                _currentState?.Tick(Time.deltaTime);
            }
        }

        public void ChangeState(IUnitState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter();
        }

        public void TakeDamage(int damage, UnitController attacker)
        {
            if (_isDead)
            {
                return;
            } 
            Stats.TakeDamage(damage);
            View.OnDamaged(Stats.CurrentHP, Stats.MaxHP);
            if (!Stats.IsAlive)
            {
                Die(attacker);
            }
        }

        public bool CheckForUnitTransfer()
        {
            return TransferChecker.CheckForEnemyTransfer(this, _gameConfig.mutationConfig);
        }

        private void Die(UnitController killer)
        {
            _isDead = true;
            ChangeState(new DeadState());
            GameEvents.RaiseUnitKilled(killer, this);
            View.PlayDeathAnimation(() => Destroy(gameObject));
        }

        private void OnValidate()
        {
            ApplyIdentity.Apply(this, _identity, unitSettings, out maxSize );
            gameObject.name = _identity.ToString();
        }
    }
}
