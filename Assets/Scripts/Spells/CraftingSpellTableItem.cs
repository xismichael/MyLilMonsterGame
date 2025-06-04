using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingSpellTableItem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public SpellCraftingManager spellCraftingManager;
    public GameObject icon;
    public TextMeshProUGUI manacost;
    public TextMeshProUGUI damage;
    public GameObject highlight;
    public Spell spell;
    float last_text_update;
    const float UPDATE_DELAY = 1;
    public int id = 0;
    public string field = "table";
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
        if (!spellCraftingManager.selected)
        {
            spellCraftingManager.IdSelected = id;
            spellCraftingManager.TypeSelected = type;
            spellCraftingManager.FieldSelected = field;
            spellCraftingManager.selected = true;
            return;
        }
    }

    public void SetEmpty()
    {
        this.spell = null;
        icon.GetComponent<Image>().sprite = null;
        manacost.text = "n/a";
        damage.text = "n/a";
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
}
