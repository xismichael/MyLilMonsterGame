using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;   // ← You forgot this

public class RelicRewardUIManager : MonoBehaviour
{
    public PlayerController player;
    public RelicRewardUI[] relicRewardUIs;
    private int relicRewardAmount = 3;
    public List<Relic> unownedRelics = new List<Relic> { };

    public string RelicSelected;

    public bool relicClaimed = false;

    void Start()
    {
        relicRewardAmount = 3;
    }

    void update()
    {

    }

    public void SetRelicUI(Relic relic, RelicRewardUI relicUI)
    {
        relicUI.SetDisplayImage(relic.SpriteIndex);
        //relicUI.SetName(relic.Name);
        relicUI.SetName("Super Sonic Healing");
        relicUI.SetDescription(relic.Description);
    }

    public void RewardScreenAction()
    {
        gameObject.SetActive(true);
        SetAllNotActive();
        relicClaimed = false;
        unownedRelics = player.relicManager.GetUnownedRelics(relicRewardAmount);
        Debug.Log("relicRewardAmount: " + relicRewardAmount);
        //Debug.Log(unownedRelics.Count);
        int actualCount = Mathf.Min(unownedRelics.Count, relicRewardAmount);
        Debug.Log($"The actualCount::::::: {unownedRelics.Count} ======= The relicRewardAmount:::::: {relicRewardAmount}");
        for (int i = 0; i < actualCount; i++)
        {
            relicRewardUIs[i].gameObject.SetActive(true);
            SetRelicUI(unownedRelics[i], relicRewardUIs[i]);
        }
    }

    public void SetAllNotActive()
    {
        foreach (RelicRewardUI relicRewardUI in relicRewardUIs)
        {
            relicRewardUI.gameObject.SetActive(false);
        }
    }

    public void OnButtonClick()
    {
        gameObject.SetActive(false);
        player.relicManager.AddRelic(RelicSelected);
        relicClaimed = true;
        //RewardScreenManager.Instance.texts[2].text = "NEXT WAVE";
        EnemySpawner.Instance.SpawnNextWave();
    }

    public void SetAllSelectedFalseExceptOne(int exceptionIndex)
    {
        for (int i = 0; i < relicRewardAmount; i++)
        {
            if (i == exceptionIndex) continue;
            relicRewardUIs[i].selected = false;
        }
    }
}
