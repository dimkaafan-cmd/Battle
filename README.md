#

## Содержание
- [Описание проекта](#описание-проекта)
- [Уникальная фишка: Система мутаций](#уникальная-фишка-система-мутаций)
- [Архитектура](#архитектура)
- [Структура проекта](#структура-проекта)
- [Ключевые решения](#ключевые-решения)
- [Система характеристик юнитов](#система-характеристик-юнитов)
- [Симуляция боя](#симуляция-боя)
- [Производительность](#производительность)
- [Как запустить](#как-запустить)
- [Расширение проекта](#расширение-проекта)
- [Затраченное время](#затраченное-время)

---

## Описание проекта

Мобильный симулятор сражений: две армии по 20 юнитов сражаются до полного уничтожения одной из сторон. Юниты — кубы и сферы разного размера и цвета — генерируются случайно перед каждым боем. После победы одной из сторон игрок возвращается в главное меню.
Дополнительно для поля боя:
    - Перед боем юниты можно двигать вруную (мышкой). 
    - Поле боя, как до боя, так и во время, можно повернуть на любой угол (мышкой), приблизить/удалить - колёсико мышки. 

**Платформа:** Пк, Mobile (Android / iOS)  
**Движок:** Unity 
**Язык:** C#

---

## Уникальная фишка: Система мутаций

### Идея
    - Когда юнит убивает врага, он **поглощает часть его характеристик** — «заражается» силой побеждённого. Сильный юнит становится ещё сильнее, меняется визуально.
    - При определенном уровне мутаций и разности уровней мутаций юнита и врага, юнит не убивает врага, а перетягивает его в свою команду.

### Как работает
- При смерти юнита его убийца получает `MutationBonus`:
  - `+15%` от ATK убитого
  - `+10%` от HP убитого (восстанавливается как бонусное HP)
- Каждая мутация визуально отражается: юнит немного **увеличивается в масштабе** и **смешивает свой цвет с цветом жертвы**
- Ограничение на количество мутаций **5 стеков мутации** на юнита — чтобы не было абсолютного доминирования
- Над юнитом отображается счётчик мутаций (💀 x N)
- Когда количество мутаций атакующего равно 5 и разность уровней мутаций между атакующим и атакуемым больше 2, враг переходит в команду юнита.

### Почему это выделяет игру
1. **Визуальная читаемость** — сразу видно «ветеранов» по их размеру и смешанному цвету
2. **Непредсказуемость** — даже меньшая армия может выиграть, если один юнит начнёт цепочку убийств
3. **Масштабируемость** — система `IMutation` позволяет добавлять новые типы мутаций без изменения существующего кода

---

## Архитектура

### Выбор подхода: MVC + ScriptableObjects
- разделение данных (Model), визуала (View) и логики (Controller) делает каждый слой независимо тестируемым и заменяемым

**ScriptableObjects для конфигурации:**
- Модификаторы цвета, формы, размера — скриптабл-обжект (SO) для сохранения
- SO автоматически сериализуются и видны в инспекторе


### Слои архитектуры

```
┌─────────────────────────────────────────────────────┐
│                     View Layer                      │
│   UnitView  │  ArmyView  │  BattleHUD  │  MenuView  │
└──────────────────────┬──────────────────────────────┘
                       │ events / callbacks
┌──────────────────────▼───────────────────────────────┐
│                  Controller Layer                    │
│  UnitController  │  BattleController  │  ArmySpawner │
└──────────────────────┬───────────────────────────────┘
                       │ reads / writes
┌──────────────────────▼──────────────────────────────┐
│                    Model Layer                      │
│   UnitStats  │  ArmyModel  │  BattleResult          │
└──────────────────────┬──────────────────────────────┘
                       │ configured by
┌──────────────────────▼──────────────────────────────┐
│               ScriptableObject Data Layer           │
│  UnitConfig  │  ShapeModifier  │  SizeModifier  │   │
│  ColorModifier  │  MutationConfig                   │
└─────────────────────────────────────────────────────┘
```

---

## Структура проекта

```
Assets/
├── Scripts/
│   ├── Data/                          # ScriptableObjects — только данные
│   │   ├── UnitConfig.cs              # Базовые характеристики юнита
│   │   ├── ShapeModifier.cs           # Модификатор формы (Cube / Sphere)
│   │   ├── SizeModifier.cs            # Модификатор размера (Big / Small)
│   │   ├── ColorModifier.cs           # Модификатор цвета (Red / Green / Blue)
│   │   ├── MutationConfig.cs          # Настройки системы мутаций
│   │   ├── UnitSettings.cs            # Настройки визуала юнита (цвет, форма(меш), материал, размер)
│   │   └── GameConfig.cs              # Комбинация модификаторов настроек формы, размеров, цветов
│   │
│   ├── Model/                         
│   │   ├── UnitStats.cs               # Текущие характеристики юнита
│   │   ├── UnitIdentity.cs            # Форма + Размер + Цвет юнита
│   │   ├── ArmyModel.cs               # Список живых юнитов, события победы
│   │   └── BattleResult.cs            # Итог боя (победитель, статистика)
│   │
│   ├── View/                          # MonoBehaviour — только визуал
│   │   ├── UnitView.cs                # Рендер, движение, анимация мутации
│   │   ├── ArmyView.cs                # Расстановка юнитов на сцене
│   │   ├── BattleHUD.cs               # UI: кнопки, таймер, счётчики
│   │   └── UnitHealthView.cs          # Визуальный эффект текущего здоровья юнита
│   │
│   ├── Controller/                    # Логика, оркестрация
│   │   ├── UnitController.cs          # FSM юнита: Move → Attack → Dead
│   │   ├── BattleController.cs        # Управляет симуляцией, проверяет победу
│   │   ├── ArmySpawner.cs             # Генерация случайных армий
│   │   ├── TargetSelector.cs          # Стратегия выбора цели
│   │   └── TransferChecker.cs         # Проверка возможности перетягивания юнита в свою команду
│   │
│   ├── StateMachine/                  # FSM юнита
│   │   ├── IUnitState.cs              # Интерфейс состояния
│   │   ├── IdleState.cs               бездействие на заданное время
│   │   ├── MoveToTargetState.cs       движение к цели пока не дойдем до цели, либо не столкнемся с юнитом своей команды
│   │   ├── AttackState.cs             атака цели
│   │   └── DeadState.cs
│   │   └── MoveToDir.cs               движение в случайном направлениии, перпендикулярном исходному
│   │
│   ├── Mutation/                      # Уникальная фишка
│   │   ├── IMutation.cs               # Интерфейс мутации
│   │   ├── MutationSystem.cs          # Применяет мутации при смерти врага
│   │   ├── StatAbsorptionMutation.cs  # Конкретная мутация: поглощение ATK/HP
│   │   └── MutationStack.cs           # Стек мутаций на юните (макс 5)
│   │
│   ├── Factory/
│   │   └── UnitFactory.cs             # Создаёт UnitStats из набора модификаторов
│   │
│   └── Utils/
│       └── GameEvents.cs              # Статические C# events для связи слоёв
│
├── ScriptableObjects/
│   ├── Shapes/
│   │   └── UnitSettings.asset         # визуальные свойства  - меш формы для данного типа формы
│   │  
│   ├── Sizes/
│   │   ├── BigModifier.asset
│   │   └── SmallModifier.asset
│   ├── Colors/
│   │   ├── RedModifier.asset
│   │   ├── GreenModifier.asset
│   │   └── BlueModifier.asset
│   └── MutationConfig.asset
│
├── Prefabs/
│   └─── Units/
│      └── CommonUnit.prefab
│      
│
└── Scenes/
    └── MainScene.unity
```

---

## Решения

### 1. ScriptableObjects как модификаторы

Каждый атрибут юнита — отдельный SO с дельтами характеристик. `UnitFactory` принимает набор SO и вычисляет финальные `UnitStats`:


### 2. FSM для юнита

Простая конечная машина состояний без внешних библиотек:

```
Idle ──► FindTarget ──► MoveToTarget ──►Проверка возможности двигаться ──► AttackTarget
              ▲               │                         │                      │
              └───────────────┘                         │                  (target dead)
              ▲                                      MoveToDir                  │
              └─────────────────────────────────────────┘                   FindTarget
```

`UnitController.Update()` делегирует `_currentState.Tick()`. Переход — `ChangeState(newState)`. Нет `if/else` цепочек в Update.

### 3. Выбор цели

Используется стратегия **ближайший враг** — `TargetSelector.GetNearest()` итерирует список живых врагов и возвращает с минимальным расстоянием. Для 20 юнитов это O(n) и не является узким местом.

### 4. Движение до цели
 При движении каждого юнита проверяем расстояние от него до всех юнитов своей команды и до целевого юнита, с учетом их реальных размеров (для простоты берется максимальный размер ограничивающего меш параллелипипида). При стокновении проверяемого юнитиа с каким-то другим:
    - если другой юнит цель (враг), то переход в состояние атаки
    - если другой юнит из своей команды 
        - если его догнали, то двигаем проверяемый юнит определенное время перпендикулярно исходному направлению, чтобы избежать пробок
        - юнит догнал проверяемый - то продолжаем движение в исходном направлении

### 5. Система мутаций (IMutation)

```csharp
public interface IMutation
{
    string MutationName { get; }
    void Apply(UnitStats killer, UnitStats victim);
}

public class StatAbsorptionMutation : IMutation
{
    public string MutationName => "Stat Absorption";
    
    public void Apply(UnitStats killer, UnitStats victim)
    {
        killer.ATK    += Mathf.RoundToInt(victim.ATK * 0.15f);
        killer.MaxHP  += Mathf.RoundToInt(victim.MaxHP * 0.10f);
        killer.CurrentHP += Mathf.RoundToInt(victim.MaxHP * 0.10f); // восстановление
    }
}

// MutationSystem.cs — вызывается из BattleController при смерти юнита
public class MutationSystem
{
    private readonly MutationConfig _config;
    private readonly List<IMutation> _mutations;

    public MutationSystem(MutationConfig config)
    {
        _config    = config;
        _mutations = new List<IMutation>
        {
            new StatAbsorptionMutation(config)
            // Новые мутации регистрируются здесь — MutationSystem не трогаем
        };
    }
    
    public void OnUnitKilled(UnitController killer, UnitController victim)
    {
        if (killer.MutationStack.Count >= _config.maxMutationStacks) return;
        
        foreach (var mutation in _mutations)
            mutation.Apply(killer.Stats, victim.Stats);
        
        killer.MutationStack.Add(victim.Identity);
        killer.View.PlayMutationEffect(victim.Identity.color);
    }
}
```

### 6. Связь слоёв через C# Events

Никаких `FindObjectOfType` или `Singleton` для связи View и Controller:

```csharp
// GameEvents.cs
public static class GameEvents
{
    public static event Action<ArmyType> OnArmyDefeated;
    public static event Action<UnitController, UnitController> OnUnitKilled; // killer, victim
    public static event Action OnBattleStarted;
    public static event Action OnRandomizeRequested;
    public static event Action<UnitController> OnUnitTransfer; - переход юнита в другую команду
}
```

`BattleHUD` подписывается на `OnArmyDefeated` для показа экрана победы. `MutationSystem` подписывается на `OnUnitKilled`. Слои не знают друг о друге.

---

## Система характеристик юнитов

| Параметр | Базовое значение |
|----------|-----------------|
| HP       | 100             |
| ATK      | 10              |
| SPEED    | 10              |
| ATKSPD   | 1               |

| Форма  | HP    | ATK |
|--------|-------|-----|
| CUBE   | +100  | +10 |
| SPHERE | +50   | +20 |

| Размер | HP  |
|--------|-----|
| BIG    | +50 |
| SMALL  | −50 |

| Цвет  | HP   | ATK | SPEED | ATKSPD |
|-------|------|-----|-------|--------|
| BLUE  | —    | −15 | +10   | +4     |
| GREEN | −100 | +20 | −5    | —      |
| RED   | +200 | +40 | −9    | —      |

> **ATKSPD** — задержка между атаками в секундах (меньше = быстрее). Базовое значение 1.0с.

---

## Симуляция боя

### Расстановка
- Армия 1: выровнена по левому краю поля, смотрит вправо
- Армия 2: выровнена по правому краю поля, смотрит влево
- Юниты расставляются в 4 ряда по 5 с небольшим случайным смещением (±0.3 units) для органичности

### Цикл боя
```
BattleController.StartBattle()
    └─► каждый UnitController запускает FSM
            │
            ▼
       [MoveToTarget] ──► преграда? ──► достиг цели? ──► [Attack] ──► цель мертва? ──► [FindTarget]
                             │                              │
                          [MoveToDir]               каждые AtkSpd секунд
            ▲                                               │
            └────────────────┘                       наносит ATK урона
                                                            │
                                                цель.HP <= 0 ──► GameEvents.OnUnitKilled
                                                                      │
                                                               MutationSystem.Apply()
                                                               ArmyModel.RemoveUnit()
                                                               UnitView.PlayDeathAnim()
```

### Проверка победы
`BattleController` слушает `ArmyModel.OnUnitRemoved`. После каждого удаления проверяет `army.IsDefeated()`. При `true` — `GameEvents.OnArmyDefeated(winner)` → переход в главное меню.


---

## Как запустить

1. Клонировать репозиторий https://github.com/dimkaafan-cmd/Battle
2. Открыть в **Unity 2022.3 LTS** (или новее)
3. Открыть сцену `Assets/Scenes/MainScene.unity`
4. Нажать Play — проект запускается без дополнительных зависимостей

---

## Расширение проекта

### Добавить новый цвет (например, PURPLE)
1. В Unity: `Assets/ScriptableObjects/Colors` → ПКМ → `Create → BattleClash → Color Modifier`
2. Заполнить поля в инспекторе (цвет, дельты характеристик)
3. Добавить новый asset в массив доступных цветов в `ArmySpawner`

### Добавить новую форму (например, CYLINDER)
1. Создать `CylinderModifier.asset` аналогично цвету
2. Создать/назначить prefab с соответствующей геометрией
3. Зарегистрировать в `ArmySpawner.shapeModifiers[]`

### Добавить новый тип мутации
1. Создать класс, реализующий `IMutation`
2. Добавить в коде в список мутаций

---

## Затраченное время

| Этап | Время |
|------|-------|
| Анализ ТЗ, проектирование архитектуры | ~3 ч |
| Настройка проекта, ScriptableObjects, UnitFactory | ~2 ч |
| UnitController (FSM), движение, атака | ~4 ч |
| BattleController, ArmySpawner, расстановка | ~3 ч |
| Система мутаций + визуальный эффект | ~4 ч |
| UI (главное меню, HUD, экран победы) | ~1 ч |
| Полировка, тестирование, фиксы | ~4 ч |
| README | ~1 ч |
| **Итого** | **~ 20 ч** |
