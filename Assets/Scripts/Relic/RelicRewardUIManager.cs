using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;   // ← You forgot this

public class RelicRewardUIManager : MonoBehaviour
{
    public PlayerController player;
    public RelicRewardUI[] relicRewardUIs;
    public int relicRewardAmount;
    public List<Relic> unownedRelics;

    public int selectionIndex;
    public string RelicSelected;

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
        relicUI.SetName(relic.Name);
        relicUI.SetDescription(relic.Description);
    }

    public void RewardScreenCallback()
    {
        gameObject.SetActive(true);
        SetAllNotActive();
        unownedRelics = player.relicManager.GetUnownedRelics(relicRewardAmount);

        int actualCount = Mathf.Min(unownedRelics.Count, relicRewardUIs.Length);
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
