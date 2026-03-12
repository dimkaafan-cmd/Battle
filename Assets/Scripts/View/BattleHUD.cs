using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Scripts.Controller;

public class BattleHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BattleController battleController;

    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button randomizeButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button resetView;

    [Header("Victory Panel")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private TextMeshProUGUI victoryText;

    [SerializeField] private float autoReturnDelay = 3f;

    private Vector3 _battleOriginPos;

    private void Awake()
    {
        _battleOriginPos = battleController.transform.position;
        randomizeButton?.onClick.AddListener(OnRandomizeClicked);
        menuButton?.onClick.AddListener(GoToMenu);
        resetView?.onClick.AddListener(ResetView);
        startButton?.onClick.AddListener(OnStartClicked);
        victoryPanel?.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        GameEvents.OnArmyDefeated  += HandleArmyDefeated;
        GameEvents.OnBattleStarted += HandleBattleStarted;
    }

    private void OnDisable()
    {
        GameEvents.OnArmyDefeated  -= HandleArmyDefeated;
        GameEvents.OnBattleStarted -= HandleBattleStarted;
    }

    private void OnStartClicked()    => battleController.StartBattle();
    private void OnRandomizeClicked()
    {
        ResetView();
        battleController.RandomizeAndRestart();
        victoryPanel?.SetActive(false);
        startButton?.gameObject.SetActive(true);
    }

    private void HandleBattleStarted()
    {
        startButton?.gameObject.SetActive(false);
        randomizeButton?.gameObject.SetActive(false);
        victoryPanel?.SetActive(false);
    }

    private void HandleArmyDefeated(ArmyType winner)
    {
        victoryPanel?.SetActive(true);
        if (victoryText  != null) victoryText.text = $"{winner} победила!";

        randomizeButton?.gameObject.SetActive(true);
        Invoke(nameof(GoToMenu), autoReturnDelay);
    }

    private void GoToMenu()
    {
        //GameEvents.ClearAllListeners();
         victoryPanel?.SetActive(false);
    }

    private void ResetView()
    {
        battleController.transform.rotation = Quaternion.identity;
        battleController.transform.position = _battleOriginPos;
    }
}
