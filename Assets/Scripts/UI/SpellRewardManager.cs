using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class SpellRewardManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public SpellUI randomSpellUI;
    public TMP_Text randomSpellDescription;

    private SpellCaster spellCaster;
    public bool spellAccepted;
    void Start()
    {
        spellCaster = GameManager.Instance.player.GetComponent<PlayerController>().spellcaster;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetSpellUI(Spell randomSpell)
    {
        randomSpellUI.SetSpell(randomSpell);
    }

    public void SetSpellDescription(string spellDescription)
    {
        randomSpellDescription.text = spellDescription;
    }

    public void AcceptSpell(Spell spell)
    {
        if (!spellAccepted)
        {
            spellCaster.AddSpell(spell);
            spellAccepted = true;
        }
    }
}
