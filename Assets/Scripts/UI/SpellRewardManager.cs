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
        randomSpellUI.SetSpell(randomSpell, -1);
        this.spell = randomSpell;
    }

    public void SetSpellDescription(Spell randomSpell)
    {
        string definitionText = randomSpell.GetFullName() + "\n";
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

    public void SetActive()
    {
        gameObject.SetActive(true);
    }

    public void SetDeactive()
    {
        gameObject.SetActive(false);
    }
}
