using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SpellRewardOptionUI : MonoBehaviour
{
    public SpellUI spellUI;
    public TMP_Text spellDescription;
    public Button acceptButton;

    private Spell spellData;

    public void Setup(Spell spell, System.Action onAccept)
    {
        spellData = spell;
        spellUI.SetSpell(spell, -1);

        string definitionText = spell.GetFullName() + "\n";
        foreach (var pair in spell.GetDefinitionList())
        {
            definitionText += $"{pair.Key}: {pair.Value}\n";
        }
        spellDescription.text = definitionText;

        acceptButton.onClick.RemoveAllListeners();
        acceptButton.onClick.AddListener(() => onAccept?.Invoke());
    }
}
