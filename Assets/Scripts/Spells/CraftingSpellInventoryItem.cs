using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingSpellInventoryItem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public SpellCraftingManager spellCraftingManager;
    public GameObject icon;
    public TextMeshProUGUI manacost;
    public TextMeshProUGUI damage;
    public GameObject highlight;
    public Spell spell = null;
    float last_text_update;
    const float UPDATE_DELAY = 1;
    public int id;
    public string field = "inventory";
    public string type = "spell";


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (spellCraftingManager.selected && id == spellCraftingManager.IdSelected && type == spellCraftingManager.TypeSelected && field == spellCraftingManager.FieldSelected) highlight.SetActive(true);
        else highlight.SetActive(false);
        if (spell == null) return;
        if (Time.time > last_text_update + UPDATE_DELAY)
        {
            manacost.text = spell.GetManaCost().ToString();
            damage.text = spell.GetDamage().ToString();
            last_text_update = Time.time;
        }


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
            //if you click the same icon
            if (id == spellCraftingManager.IdSelected)
            {
                spellCraftingManager.selected = false;
                return;
            }

            //if you click within the table
            spellCraftingManager.SpellInventorySwap(id);
            return;
        }

        //inventory crafting table sqap
        if (spellCraftingManager.FieldSelected == "table")
        {
            spellCraftingManager.SpellInventoryTableAction(id);
            return;
        }

        //inventory current swap
        spellCraftingManager.SpellInventoryCurrentAction(id, spellCraftingManager.IdSelected);

    }

    public string GetName()
    {
        if (spell == null) return "empty spell";
        return spell.GetFullName();
    }
    public void SetSpell(Spell spell)
    {
        if (spell == null)
        {
            SetEmpty();
            return;
        }
        this.spell = spell;
        GameManager.Instance.spellIconManager.PlaceSprite(spell.GetIcon(), icon.GetComponent<Image>());
    }

    public void SetEmpty()
    {
        this.spell = null;
        icon.GetComponent<Image>().sprite = null;
        manacost.text = "n/a";
        damage.text = "n/a";
    }
}
