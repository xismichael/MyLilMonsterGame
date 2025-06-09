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
    //public GameObject spellRewardOptionPrefab;
    //public Transform rewardOptionParent;
    public bool isModifierReward;
    public string modifierKey;

    public List<SpellRewardOptionUI> rewardSlots = new List<SpellRewardOptionUI>();


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
    //public void ShowRewardOptions(List<Spell> spellOptions)
    //{
    //    // Clear old UI
    //    foreach (Transform child in rewardOptionParent)
    //        Destroy(child.gameObject);

    //    spellAccepted = false;
    //    spellCaster = GameManager.Instance.player.GetComponent<PlayerController>().spellcaster;

    //    foreach (Spell spell in spellOptions)
    //    {
    //        GameObject optionObj = Instantiate(spellRewardOptionPrefab, rewardOptionParent);
    //        optionObj.GetComponent<SpellRewardOptionUI>().Setup(spell, AcceptSpell);
    //    }

    //    SetActive();
    //}
    public void ShowRewardOptions(List<Spell> spellOptions, List<bool> isModifierFlags, List<string> modifierKeys)
    {
        for (int i = 0; i < rewardSlots.Count; i++)
        {
            if (i < spellOptions.Count)
            {
                rewardSlots[i].gameObject.SetActive(true);
                Spell spell = spellOptions[i];
                bool isMod = isModifierFlags[i];
                string modKey = modifierKeys[i];

                rewardSlots[i].Setup(spell, () => AcceptSpell(spell, isMod, modKey));
            }
            else
            {
                rewardSlots[i].gameObject.SetActive(false);
            }
        }

        SetActive();
    }

    //public void AcceptSpell(Spell spell)
    //{
    //    //if (!spellAccepted)
    //    //{
    //    //    spellCaster = GameManager.Instance.player.GetComponent<PlayerController>().spellcaster;
    //    //    if (spellCaster.GetCurrentSpellAmount() < spellCaster.maxSpellCount)
    //    //    {
    //    //        spellCaster.AddSpell(spell);
    //    //        spellAccepted = true;
    //    //    }
    //    //}
    //    if (spellAccepted) return;

    //    var spellCaster = GameManager.Instance.player.GetComponent<PlayerController>().spellcaster;
    //    var manager = GameManager.Instance.player.GetComponent<PlayerController>().spellCraftingManager;

    //    if (isModifierReward && !string.IsNullOrEmpty(modifierKey))
    //    {
    //        manager.AddModifierToInventory(modifierKey);
    //    }
    //    else if (spellCaster.GetCurrentSpellAmount() < spellCaster.maxSpellCount)
    //    {
    //        spellCaster.AddSpell(spell);
    //        manager.AddSpellToInventory(spell);
    //    }

    //    spellAccepted = true;
    //}
    public void AcceptSpell(Spell spell, bool isModifier, string modifierKey)
    {
        //if (spellAccepted) return;

        var spellCaster = GameManager.Instance.player.GetComponent<PlayerController>().spellcaster;
        var manager = GameManager.Instance.player.GetComponent<PlayerController>().spellCraftingManager;

        if (isModifier && !string.IsNullOrEmpty(modifierKey))
        {
            manager.AddModifierToInventory(modifierKey);
        }
        else if (spellCaster.GetCurrentSpellAmount() < spellCaster.maxSpellCount)
        {
            manager.AddSpellToInventory(spell);
        }

        //spellAccepted = true;
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
