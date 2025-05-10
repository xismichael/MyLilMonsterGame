using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SpellUIContainer : MonoBehaviour
{
    public GameObject[] spellUIs;
    public PlayerController player;
    void Start()
    { }

    // Update is called once per frame
    void Update()
    {
        SpellUI temp = spellUIs[player.spellcaster.spellCastIndex].GetComponent<SpellUI>();
        if (temp != null) temp.highlight.SetActive(true);
    }

    public void LoadUI(List<Spell> spells)
    {
        for (int i = 0; i < spells.Count; i++)
        {
            spellUIs[i].GetComponent<SpellUI>().SetSpell(spells[i], i);
            ActivateUI(i);
        }
    }

    public void DeactiveUI(int index)
    {
        spellUIs[index].SetActive(false);
    }

    public void ActivateUI(int index)
    {
        spellUIs[index].SetActive(true);
    }

    public void DeactiveAllUI()
    {
        for (int i = 0; i < spellUIs.Length; i++)
        {
            DeactiveUI(i);
        }
    }

    public void RewardscreenShift()
    {
        gameObject.transform.position = new Vector3(750, 400, 0);
        foreach (GameObject spellUI in spellUIs)
        {
            spellUI.GetComponent<SpellUI>().clicked = false;
        }
    }

    public void GameplayShift()
    {
        gameObject.transform.position = new Vector3(32, 32, 0);
    }

    public void OpenAllDropButton()
    {
        for (int i = 0; i < spellUIs.Length; i++)
        {
            spellUIs[i].GetComponent<SpellUI>().dropbutton.SetActive(true);
        }
    }

    public void closeAllDropButton()
    {
        for (int i = 0; i < spellUIs.Length; i++)
        {
            spellUIs[i].GetComponent<SpellUI>().dropbutton.SetActive(true);
        }
    }
}
