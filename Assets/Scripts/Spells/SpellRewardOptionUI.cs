using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Data;

public class SpellRewardOptionUI : MonoBehaviour
{
    public SpellUI spellUI;
    public TMP_Text spellDescription;
    public Button acceptButton;
    private bool alreadyAccepted = false;

    private Spell spellData;

    public void Setup(Spell spell, System.Action onAccept)
    {
        alreadyAccepted = false;
        acceptButton.interactable = true;
        acceptButton.GetComponentInChildren<TMP_Text>().text = "Accept";

        spellData = spell;
        spellUI.SetSpell(spell, -1);

        string definitionText = spell.GetFullName() + "\n";
        foreach (var pair in spell.GetDefinitionList())
        {
            definitionText += $"{pair.Key}: {pair.Value}\n";
        }
        spellDescription.text = definitionText;

        acceptButton.onClick.RemoveAllListeners();
        acceptButton.onClick.AddListener(() =>
        {
            if (alreadyAccepted) return;
            onAccept?.Invoke();
            acceptButton.interactable = false;
            acceptButton.GetComponentInChildren<TMP_Text>().text = "Accepted";
            alreadyAccepted = true;
        });
    }
}
