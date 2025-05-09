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
    private Spell spell;
    public bool spellAccepted;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetSpellUI(Spell randomSpell)
    {
        randomSpellUI.SetSpell(randomSpell);
        this.spell = randomSpell;
    }

    public void SetSpellDescription(Spell randomSpell)
    {
        string definitionText = "";
        List<KeyValuePair<string, string>> definition = randomSpell.GetDefinitionList();
        foreach (KeyValuePair<string, string> pair in definition)
        {
            definitionText += $"{pair.Key}: {pair.Value}\n";
        }
        randomSpellDescription.text = definitionText;
    }

    public void AcceptSpell()
    {
        if (!spellAccepted)
        {
            spellCaster = GameManager.Instance.player.GetComponent<PlayerController>().spellcaster;
            spellCaster.AddSpell(spell);
            spellAccepted = true;
        }
    }
}
