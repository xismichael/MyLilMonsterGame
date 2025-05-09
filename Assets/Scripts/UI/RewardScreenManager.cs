using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RewardScreenManager : MonoBehaviour
{
    public GameObject rewardUI;

    public TMP_Text[] texts;

    public Button nextStageButton;

    private bool ScreenActivate;

    public SpellRewardManager spellRewardManager;

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
            texts[1].text = "WAVE FINISHED";
            texts[0].text = $"Wave {EnemySpawner.CurrentWaveNumber}\n" +
                                    $"Kills: {EnemySpawner.Instance.currentWaveEnemiesKilled}\n" +
                                    $"Damage Taken: {EnemySpawner.Instance.currentWaveDamageTaken}";


            Spell randomSpell = SpellBuilder.Instance.CreateRandomSpell(GameManager.Instance.player.GetComponent<PlayerController>().spellcaster);
            spellRewardManager.spellAccepted = false;
            spellRewardManager.SetSpellDescription(randomSpell);
            spellRewardManager.SetSpellUI(randomSpell);




            nextStageButton.onClick.AddListener(WaveEndButtonAction);
            texts[2].text = "NEXT WAVE";
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
                texts[1].text = "YOU DIED";
            }
            else
            {
                texts[1].text = "VICTORY";
            }

            rewardUI.SetActive(true);
            texts[0].text = $"Wave {EnemySpawner.CurrentWaveNumber}\n" +
                                $"Total Kills: {EnemySpawner.Instance.TotalEnemiesKilled}\n" +
                                $"Total Damage Taken: {EnemySpawner.Instance.TotalDamageTaken}";
            nextStageButton.onClick.AddListener(GameOverButtonAction);
            texts[2].text = "RESTART";
            ScreenActivate = true;
        }

    }

    public void GameOverButtonAction()
    {
        rewardUI.SetActive(false);
        EnemySpawner.Instance.restartScreen();
    }


}
