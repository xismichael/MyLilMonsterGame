using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingSpellCurrentItem : MonoBehaviour
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
    public int id;
    public string selectedField = "current";
    public string type = "spell";


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
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

    }
}
