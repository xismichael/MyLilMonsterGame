using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using TMPro;

public class SpellCraftingManager : MonoBehaviour
{

    //player
    public PlayerController player;
    //inventory
    private int startingXPosition = 350;
    private int startingYPosition = 690;
    private int incrementSpacing = 150;
    private int itemsPerRow = 4;
    private int itemsPerColumn = 3;
    private int maxItems = 12;
    private List<Spell> spellsInventory = new List<Spell> { };
    private List<string> modifiersInventory = new List<string> { "lala", "lala", "lala" };
    public string currentInventory = "spell";

    //name displaying
    public TMP_Text DisplaySpellText;
    public TMP_Text InventoryButtonText;


    //for what is currently selected
    public string TypeSelected;
    public int IdSelected;
    public string FieldSelected;

    //inventory items to be created
    private List<GameObject> gameObjectInventoryItems = new List<GameObject> { };

    //table items
    public CraftingSpellTableItem craftingSpellTableItem;
    public CraftingModifierTableItem craftingModifierTableItem;

    //current spells
    public CraftingSpellCurrentItem[] craftingSpellCurrentItems;

    public GameObject CraftingSpellInventoryItemPrefab;
    public GameObject CraftingModifierInventoryItemPrefab;



    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void initialize()
    {
        gameObject.SetActive(true);
        currentInventory = "spell";
        MakeSpellInventory();
        MakeCurrentHeldSpells();
    }

    public void AddSpellToInventory(Spell spell)
    {
        spellsInventory.Add(spell);
    }

    public void AddModifierToInventory(string modifier)
    {
        modifiersInventory.Add(modifier);
    }

    public void MakeSpellInventory()
    {
        DeleteInventory();
        currentInventory = "spell";
        InventoryButtonText.text = currentInventory;
        int currentItemCount = 0;
        foreach (Spell spell in spellsInventory)
        {
            //Debug.Log("CurrentCount: " + currentItemCount);
            //Debug.Log("x position: " + (startingXPosition + (currentItemCount % itemsPerRow) * incrementSpacing) + " y position: " + (startingYPosition + (currentItemCount / itemsPerColumn) * incrementSpacing));
            GameObject craftingSpellInventoryItem = Instantiate(CraftingSpellInventoryItemPrefab, gameObject.transform);
            craftingSpellInventoryItem.transform.position = new Vector3((startingXPosition + (currentItemCount % itemsPerRow) * incrementSpacing), (startingYPosition - (currentItemCount / itemsPerRow) * incrementSpacing));
            //Debug.Log("realx: " + CraftingSpellInventoryItem.transform.position.x + " realy: " + CraftingSpellInventoryItem.transform.position.y);
            craftingSpellInventoryItem.GetComponent<CraftingSpellInventoryItem>().spellCraftingManager = this;
            craftingSpellInventoryItem.GetComponent<CraftingSpellInventoryItem>().SetSpell(spell);
            craftingSpellInventoryItem.GetComponent<CraftingSpellInventoryItem>().id = currentItemCount;
            gameObjectInventoryItems.Add(craftingSpellInventoryItem);
            currentItemCount++;
        }
    }

    public void MakeModiferInventory()
    {
        DeleteInventory();
        currentInventory = "modifier";
        InventoryButtonText.text = currentInventory;
        int currentItemCount = 0;
        foreach (string modifier in modifiersInventory)
        {
            //Debug.Log("CurrentCount: " + currentItemCount);
            //Debug.Log("x position: " + (startingXPosition + (currentItemCount % itemsPerRow) * incrementSpacing) + " y position: " + (startingYPosition + (currentItemCount / itemsPerColumn) * incrementSpacing));
            GameObject craftingModifierInventoryItem = Instantiate(CraftingModifierInventoryItemPrefab, gameObject.transform);
            craftingModifierInventoryItem.transform.position = new Vector3((startingXPosition + (currentItemCount % itemsPerRow) * incrementSpacing), (startingYPosition - (currentItemCount / itemsPerRow) * incrementSpacing));
            //Debug.Log("realx: " + CraftingSpellInventoryItem.transform.position.x + " realy: " + CraftingSpellInventoryItem.transform.position.y);
            craftingModifierInventoryItem.GetComponent<CraftingModifierInventoryItem>().spellCraftingManager = this;
            craftingModifierInventoryItem.GetComponent<CraftingModifierInventoryItem>().SetModifier(modifier);
            craftingModifierInventoryItem.GetComponent<CraftingModifierInventoryItem>().id = currentItemCount;
            gameObjectInventoryItems.Add(craftingModifierInventoryItem);
            currentItemCount++;
        }

    }

    public void DeleteInventory()
    {
        foreach (GameObject inventoryItem in gameObjectInventoryItems)
        {
            Destroy(inventoryItem);
        }
        gameObjectInventoryItems = new List<GameObject> { };
    }

    public void MakeCurrentHeldSpells()
    {
        if (player.spellcaster == null) return;
        for (int i = 0; i < player.spellcaster.spells.Count(); i++)
        {
            craftingSpellCurrentItems[i].SetSpell(player.spellcaster.spells[i]);
        }
    }

    public void SpellInventoryOnClick()
    {
        if (currentInventory == "spell")
        {
            MakeModiferInventory();
        }
        else if (currentInventory == "modifier")
        {
            MakeSpellInventory();
        }

    }

    public void CraftOnClick()
    {

    }

}
