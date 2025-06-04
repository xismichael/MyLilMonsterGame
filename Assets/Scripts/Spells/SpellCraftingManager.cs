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
    public bool selected = false;
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

    public void close()
    {
        gameObject.SetActive(false);
        CurrentSpellChangeOnClose();
        RemoveNullFromInventory();
        TableCloseAction();
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

    public void SpellInventorySwap(int newId)
    {
        //get the spells at each position
        Spell currentIdSpell = spellsInventory[IdSelected];
        Spell newIdSpell = spellsInventory[newId];

        //swap inventory positions
        spellsInventory[newId] = currentIdSpell;
        spellsInventory[IdSelected] = newIdSpell;

        //swap UI visual positions
        gameObjectInventoryItems[newId].GetComponent<CraftingSpellInventoryItem>().SetSpell(currentIdSpell);
        gameObjectInventoryItems[IdSelected].GetComponent<CraftingSpellInventoryItem>().SetSpell(newIdSpell);

        //reset the selected
        selected = false;

    }

    public void ModifierInventorySwap(int newId)
    {
        //get the modifier at each position
        string currentIdModifier = modifiersInventory[IdSelected];
        string newIdModifier = modifiersInventory[newId];

        //swap inventory positions
        modifiersInventory[newId] = currentIdModifier;
        modifiersInventory[IdSelected] = newIdModifier;

        //swap UI visual positions
        gameObjectInventoryItems[newId].GetComponent<CraftingModifierInventoryItem>().SetModifier(currentIdModifier);
        gameObjectInventoryItems[IdSelected].GetComponent<CraftingModifierInventoryItem>().SetModifier(newIdModifier);

        //reset the selected
        selected = false;

    }

    public void SpellInventoryTableAction(int InventoryId)
    {
        //get the spells at each position
        Spell InventorySpell = spellsInventory[InventoryId];
        Spell TableSpell = craftingSpellTableItem.spell;

        //swap inventory positions
        craftingSpellTableItem.spell = InventorySpell;
        spellsInventory[InventoryId] = TableSpell;

        //swap UI visual positions
        gameObjectInventoryItems[InventoryId].GetComponent<CraftingSpellInventoryItem>().SetSpell(TableSpell);
        craftingSpellTableItem.SetSpell(InventorySpell);

        //reset the selected
        selected = false;
    }

    public void SpellInventoryCurrentAction(int InventoryId)
    {
        //get spell at each position
        Spell InventorySpell = spellsInventory[InventoryId];
        Spell CurrentSpell = craftingSpellCurrentItems[IdSelected].spell;

        // InventorySpell swap
        spellsInventory[InventoryId] = CurrentSpell;

        //UI visual swap
        gameObjectInventoryItems[InventoryId].GetComponent<CraftingSpellInventoryItem>().SetSpell(CurrentSpell);
        craftingSpellCurrentItems[IdSelected].SetSpell(InventorySpell);
    }

    public void CurrentSpellChangeOnClose()
    {
        List<Spell> newSpells = new List<Spell> { };
        foreach (CraftingSpellCurrentItem craftingSpellCurrentItem in craftingSpellCurrentItems)
        {
            if (craftingSpellCurrentItem.spell == null) continue;
            newSpells.Add(craftingSpellCurrentItem.spell);
        }

        player.spellcaster.ReloadSpell(newSpells);
    }

    public void RemoveNullFromInventory()
    {

    }

    public void TableCloseAction()
    {

    }


}
