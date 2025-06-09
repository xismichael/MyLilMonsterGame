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

    public Button creditsButton;
    public GameObject creditsPanel;

    private bool ScreenActivate;

    public SpellRewardManager spellRewardManager;

    public RelicRewardUIManager relicRewardManager;

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
            creditsButton.gameObject.SetActive(false);
        }
    }

    public void WaveEndScreen()
    {
        creditsPanel.SetActive(false);
        if (!ScreenActivate)
        {
            rewardUI.SetActive(true);
            texts[1].text = "WAVE FINISHED";
            texts[0].text = $"Wave {EnemySpawner.CurrentWaveNumber}\n" +
                                    $"Kills: {EnemySpawner.Instance.currentWaveEnemiesKilled}\n" +
                                    $"Damage Taken: {EnemySpawner.Instance.currentWaveDamageTaken}";

            var spellCaster = GameManager.Instance.player.GetComponent<PlayerController>().spellcaster;

            List<Spell> rewards = new List<Spell>();
            List<bool> isModifiers = new List<bool>();
            List<string> modifierKeys = new List<string>();

            for (int i = 0; i < 3; i++)
            {
                bool isMod;
                string modKey;
                var reward = SpellBuilder.Instance.CreateRandomReward(spellCaster, out isMod, out modKey);

                rewards.Add(reward);
                isModifiers.Add(isMod);
                modifierKeys.Add(modKey);
            }

            spellRewardManager.ShowRewardOptions(rewards, isModifiers, modifierKeys);

            // Show them on the reward screen
            //spellRewardManager.ShowRewardOptions(rewards);
            //Spell randomSpell = SpellBuilder.Instance.CreateRandomSpell(GameManager.Instance.player.GetComponent<PlayerController>().spellcaster);

            spellRewardManager.spellAccepted = false;
            //spellRewardManager.SetSpellDescription(randomSpell);
            //spellRewardManager.SetSpellUI(randomSpell);
            GameManager.Instance.player.GetComponent<PlayerController>().spellUIContainer.OpenAllDropButton();
            GameManager.Instance.player.GetComponent<PlayerController>().spellUIContainer.RewardscreenShift();
            spellRewardManager.SetActive();

            if (EnemySpawner.CurrentWaveNumber % 3 == 0)
            // if (EnemySpawner.CurrentWaveNumber % 1 == 0) // FOR FASTER, SWIFTER TESTING!!! 
            {
                nextStageButton.onClick.AddListener(RelicRewardAction);
                texts[2].text = "RELIC CLAIM";
                creditsButton.gameObject.SetActive(false);
                ScreenActivate = true;
            }
            else
            {
                nextStageButton.onClick.AddListener(WaveEndButtonAction);
                texts[2].text = "NEXT WAVE";
                creditsButton.gameObject.SetActive(false);
                ScreenActivate = true;
            }
        }


    }

    public void RelicRewardAction()
    {
        creditsPanel.SetActive(false);
        GameManager.Instance.player.GetComponent<PlayerController>().spellUIContainer.GameplayShift();
        relicRewardManager.RewardScreenAction();
    }
    public void WaveEndButtonAction()
    {
        creditsPanel.SetActive(false);
        GameManager.Instance.player.GetComponent<PlayerController>().spellUIContainer.GameplayShift();
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

            spellRewardManager.SetDeactive();
            rewardUI.SetActive(true);
            texts[0].text = $"Wave {EnemySpawner.CurrentWaveNumber}\n" +
                                $"Total Kills: {EnemySpawner.Instance.TotalEnemiesKilled}\n" +
                                $"Total Damage Taken: {EnemySpawner.Instance.TotalDamageTaken}";
            nextStageButton.onClick.AddListener(GameOverButtonAction);
            texts[2].text = "RESTART";

            creditsButton.gameObject.SetActive(true);
            creditsButton.onClick.RemoveAllListeners();
            creditsButton.onClick.AddListener(StartCredits);
            ScreenActivate = true;

            
        }

    }

    public void GameOverButtonAction()
    {

        rewardUI.SetActive(false);
        RoleClassManager.Instance.gameObject.SetActive(true);
    }

    public void StartCredits()
    {

        creditsPanel.SetActive(true);

    }


}
