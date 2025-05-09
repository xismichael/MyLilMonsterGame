using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SpellUIContainer : MonoBehaviour
{
    public GameObject[] spellUIs;
    public PlayerController player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // we only have one spell (right now)
        // spellUIs[0].SetActive(true);
        // spellUIs[1].SetActive(true);
        // for (int i = 2; i < spellUIs.Length; ++i)
        // {
        //     spellUIs[i].SetActive(false);
        // }
    }

    // Update is called once per frame
    void Update()
    {
        spellUIs[player.spellcaster.spellCastIndex].GetComponent<SpellUI>().highlight.SetActive(true);
    }

    public void LoadUI(List<Spell> spells)
    {
        for (int i = 0; i < spells.Count; i++)
        {
            spellUIs[i].GetComponent<SpellUI>().SetSpell(spells[i]);
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
}
