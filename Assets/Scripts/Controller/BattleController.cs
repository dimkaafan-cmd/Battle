using Scripts.Data;
using UnityEngine;

namespace Scripts.Controller{
    public class BattleController : MonoBehaviour
    {
        [Header("References")]
        
        [SerializeField] private MeshRenderer startPosTeamA;
        [SerializeField] private MeshRenderer startPosTeamB;
        [SerializeField] private ArmySpawner    spawner;
        [SerializeField] private ArmyController  teamCreatorA;
        [SerializeField] private ArmyController  teamCreatorB;
        [SerializeField] private MutationConfig mutationConfig;

        private ArmyModel      _teamA;
        private ArmyModel      _teamB;
        private MutationSystem _mutations;
        private bool           _battleRunning;

        private void OnEnable() {
            GameEvents.OnUnitKilled += HandleUnitKilled;
            GameEvents.OnUnitTransfer += HandleUnitTransfer;
        }
        private void OnDisable(){
            GameEvents.OnUnitKilled -= HandleUnitKilled;
            GameEvents.OnUnitTransfer -= HandleUnitTransfer;
        }

        public bool IsBattleRunning => _battleRunning;
        public void StartBattle()
        {
            if (_battleRunning) return;
            _battleRunning = true;
            _mutations     = new MutationSystem(mutationConfig);

            _teamA = teamCreatorA.GetArmy();
            _teamB = teamCreatorB.GetArmy();

            // Устанавливаем вражеские армии после создания обеих
            foreach (var u in _teamA.Units) u.EnemyArmy = _teamB;
            foreach (var u in _teamB.Units) u.EnemyArmy = _teamA;

            _teamA.OnDefeated += () => OnBattleOver(ArmyType.TeamB);
            _teamB.OnDefeated += () => OnBattleOver(ArmyType.TeamA);

            GameEvents.RaiseBattleStarted();
        }

        public void RandomizeAndRestart()
        {
            DestroyAllUnits();
            _battleRunning = false;
            RandomiseArmy();
        }


        private void RandomiseArmy()
        {
            teamCreatorA.CleanArmy();
            teamCreatorB.CleanArmy();
            spawner.SpawnArmy(teamCreatorA.transform, ArmyType.TeamA, startPosTeamA.bounds, true);
            spawner.SpawnArmy(teamCreatorB.transform, ArmyType.TeamB, startPosTeamB.bounds, false);
        }

        private void HandleUnitKilled(UnitController killer, UnitController victim)
        {
            if (_teamA.Units.Contains(victim)) _teamA.RemoveUnit(victim);
            else                               _teamB.RemoveUnit(victim);

            if (killer != null && killer.IsAlive)
                _mutations.OnUnitKilled(killer, victim);
        }

        private void HandleUnitTransfer(UnitController transferedUnit)
        {
            var toArmy = transferedUnit.EnemyArmy;
            transferedUnit.OwnArmy.RemoveUnit(transferedUnit);
            toArmy.AddUnit(transferedUnit);
            transferedUnit.EnemyArmy = transferedUnit.OwnArmy;
            transferedUnit.OwnArmy = toArmy;
            transferedUnit.ChangeState(new IdleState(transferedUnit));
        }

        private void OnBattleOver(ArmyType winner)
        {
            _battleRunning = false;
            GameEvents.RaiseArmyDefeated(winner);
        }

        private void DestroyAllUnits()
        {
            void Destroy(ArmyModel army)
            {
                if (army == null) return;
                foreach (var u in army.Units)
                    if (u != null) UnityEngine.Object.Destroy(u.gameObject);
            }
            Destroy(_teamA);
            Destroy(_teamB);
        }
    }
}
