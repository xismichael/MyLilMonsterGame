using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingModifierTableItem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public SpellCraftingManager spellCraftingManager;
    public GameObject icon;
    public GameObject highlight;
    public string modifier = null;
    public int id = 0;
    public string type = "modifier";
    public string field = "table";


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
        if (!spellCraftingManager.selected || type != spellCraftingManager.TypeSelected)
        {
            spellCraftingManager.IdSelected = id;
            spellCraftingManager.TypeSelected = type;
            spellCraftingManager.FieldSelected = field;
            spellCraftingManager.selected = true;
            spellCraftingManager.DisplaySpellText.text = GetName();
            return;
        }

        if (field == spellCraftingManager.FieldSelected)
        {
            spellCraftingManager.selected = false;
            return;
        }

        if (spellCraftingManager.FieldSelected == "inventory")
        {
            spellCraftingManager.ModifierInventoryTableAction(spellCraftingManager.IdSelected);
            return;
        }
    }

    public void SetModifier(string newModifier)
    {
        if (newModifier == null || newModifier == "")
        {
            SetEmpty();
            return;
        }
        modifier = newModifier;
        //missing icon
    }

    public void SetEmpty()
    {
        modifier = null;
        icon.GetComponent<Image>().sprite = null;
    }

    public string GetName()
    {
        if (modifier == null || modifier == "") return "empty modifier";
        return SpellBuilder.Instance.GetSpellObject(modifier)["name"].ToString();
    }
}
