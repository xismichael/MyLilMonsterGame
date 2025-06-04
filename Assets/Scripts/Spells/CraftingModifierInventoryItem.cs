using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingModifierInventoryItem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public SpellCraftingManager spellCraftingManager;
    public GameObject icon;
    public GameObject highlight;
    public string modifier;
    public int id;
    public string type = "modifier";
    public string field = "inventory";


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (spellCraftingManager.selected && id == spellCraftingManager.IdSelected && type == spellCraftingManager.TypeSelected && field == spellCraftingManager.FieldSelected) highlight.SetActive(true);
        else highlight.SetActive(false);

    }
    public void OnClick()
    {
        if (!spellCraftingManager.selected)
        {
            spellCraftingManager.IdSelected = id;
            spellCraftingManager.TypeSelected = type;
            spellCraftingManager.FieldSelected = field;
            spellCraftingManager.selected = true;
            return;
        }
    }

    public void SetModifier(string newModifier)
    {
        modifier = newModifier;
    }
}
