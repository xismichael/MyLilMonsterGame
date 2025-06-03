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
    public string selectedField = "inventory";


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnClick()
    {

    }

    public void SetModifier(string newModifier)
    {
        modifier = newModifier;
    }
}
