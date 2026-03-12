using System;
using Scripts.Controller;

/// <summary>
/// Статический event-bus для связи между слоями без прямых зависимостей.
/// View подписывается на события Controller/Model, не зная об их существовании напрямую.
/// </summary>
public static class GameEvents
{
    /// <summary>Юнит убит. Параметры: убийца, жертва.</summary>
    public static event Action<UnitController, UnitController> OnUnitKilled;
    public static event Action<UnitController> OnUnitTransfer;

    /// <summary>Армия полностью уничтожена. Параметр: победившая сторона.</summary>
    public static event Action<ArmyType> OnArmyDefeated;

    /// <summary>Бой начался.</summary>
    public static event Action OnBattleStarted;

    /// <summary>Запрошена рандомизация армий.</summary>
    public static event Action OnRandomizeRequested;

    public static void RaiseUnitKilled(UnitController killer, UnitController victim)
        => OnUnitKilled?.Invoke(killer, victim);

        public static void RaiseUnitTransfer(UnitController transfered)
        => OnUnitTransfer?.Invoke(transfered);

    public static void RaiseArmyDefeated(ArmyType winner)
        => OnArmyDefeated?.Invoke(winner);

    public static void RaiseBattleStarted()
        => OnBattleStarted?.Invoke();

    public static void RaiseRandomizeRequested()
        => OnRandomizeRequested?.Invoke();

    /// <summary>Вызывать при выгрузке сцены, чтобы избежать утечек подписок.</summary>
    public static void ClearAllListeners()
    {
        OnUnitKilled        = null;
        OnArmyDefeated      = null;
        OnBattleStarted     = null;
        OnRandomizeRequested = null;
        OnUnitTransfer = null;
    }
}
