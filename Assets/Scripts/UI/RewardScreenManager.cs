using UnityEngine;
using TMPro;

public class RewardScreenManager : MonoBehaviour
{
    public GameObject rewardUI;
    public TMP_Text waveStatsText;
    public TMP_Text deathMessage; 

    public static RewardScreenManager Instance { get; private set; } 

    private bool shownStats = false; // tracks if stats show when wave ends

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this; 
    }

    public void ShowDeathMessage()
    {
        if (deathMessage != null)
            deathMessage.gameObject.SetActive(true); 
    }

    void Update()
    {
        if (GameManager.Instance.state == GameManager.GameState.WAVEEND)
        {
            rewardUI.SetActive(true);

            if (!shownStats)
            {
                if (waveStatsText != null)
                {
                    // display stats at end of wave
                    waveStatsText.text = $"Wave: {EnemySpawner.CurrentWaveNumber}\n" +
                                         $"Kills: {GameManager.Instance.currentWaveEnemiesKilled}\n" +
                                         $"Damage Taken: {GameManager.Instance.currentWaveDamageTaken}";
                }

                // show death message if game over
                if (GameManager.Instance.state == GameManager.GameState.GAMEOVER && deathMessage != null)
                {
                    deathMessage.gameObject.SetActive(true);
                }

                shownStats = true;
            }
        }
        else
        {
            rewardUI.SetActive(false);
            shownStats = false;

            if (waveStatsText != null)
                waveStatsText.text = "";
            if (deathMessage != null && GameManager.Instance.state != GameManager.GameState.GAMEOVER)
                deathMessage.gameObject.SetActive(false);
        }
    }

    // next wave call method
    public void NextWaveButtonPressed()
    {
        GameManager.Instance.currentWaveEnemiesKilled = 0;
        GameManager.Instance.currentWaveDamageTaken = 0;

        if (EnemySpawner.Instance != null)
        {
            EnemySpawner.Instance.SpawnNextWave(); // cleanly continues the game
        }

        rewardUI.SetActive(false); // hide reward screen after clicking 

        if (waveStatsText != null)
            waveStatsText.text = "";

        if (deathMessage != null)
            deathMessage.gameObject.SetActive(false); // ensure death message is hidden when continuing
    }
}
