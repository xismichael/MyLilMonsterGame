using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RewardScreenManager : MonoBehaviour
{
    public GameObject rewardUI;
    public TMP_Text waveStatsText;
    public TMP_Text HeaderText;

    public Button nextStageButton;
    public TMP_Text nextStageButtonText;

    private bool ScreenActivate;

    public static RewardScreenManager Instance { get; private set; }


    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
        ScreenActivate = false;
    }

    void Update()
    {
        if (GameManager.Instance.state == GameManager.GameState.WAVEEND)
        {
            WaveEndScreen();
        }

        else if (GameManager.Instance.state == GameManager.GameState.GAMEOVER)
        {
            GameOverScreen();
        }
        else
        {
            nextStageButton.onClick.RemoveAllListeners();
            ScreenActivate = false;
            rewardUI.SetActive(false);
        }
    }

    public void WaveEndScreen()
    {
        if (!ScreenActivate)
        {
            rewardUI.SetActive(true);
            HeaderText.text = "WAVE FINISHED";
            waveStatsText.text = $"Wave {EnemySpawner.CurrentWaveNumber}\n" +
                                    $"Kills: {EnemySpawner.Instance.currentWaveEnemiesKilled}\n" +
                                    $"Damage Taken: {EnemySpawner.Instance.currentWaveDamageTaken}";
            nextStageButton.onClick.AddListener(WaveEndButtonAction);
            nextStageButtonText.text = "NEXT WAVE";
            ScreenActivate = true;
        }


    }
    public void WaveEndButtonAction()
    {
        EnemySpawner.Instance.SpawnNextWave();
    }
    public void GameOverScreen()
    {
        if (!ScreenActivate)
        {

            if (GameManager.Instance.PlayerDeath)
            {
                HeaderText.text = "YOU DIED";
            }
            else
            {
                HeaderText.text = "VICTORY";
            }

            rewardUI.SetActive(true);
            waveStatsText.text = $"Wave {EnemySpawner.CurrentWaveNumber}\n" +
                                $"Total Kills: {EnemySpawner.Instance.TotalEnemiesKilled}\n" +
                                $"Total Damage Taken: {EnemySpawner.Instance.TotalDamageTaken}";
            nextStageButton.onClick.AddListener(GameOverButtonAction);
            nextStageButtonText.text = "RESTART";
            ScreenActivate = true;
        }

    }

    public void GameOverButtonAction()
    {
        Debug.Log("here");
        rewardUI.SetActive(false);
        EnemySpawner.Instance.restartScreen();
    }


}
