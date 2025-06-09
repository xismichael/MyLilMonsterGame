using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SpellBuilder : MonoBehaviour
{
    public static SpellBuilder Instance { get; private set; }

    private Dictionary<string, JObject> spellDefinitions;

    public GameObject TurretPrefab;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadSpells();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public JObject GetSpellObject(string spellName)
    {
        return spellDefinitions[spellName];
    }

    private void LoadSpells()
    {
        spellDefinitions = new Dictionary<string, JObject>();

        TextAsset jsonFile = Resources.Load<TextAsset>("spells");
        if (jsonFile == null)
        {
            Debug.LogError("spells.json not found in Resources!");
            return;
        }

        JObject root = JObject.Parse(jsonFile.text);

        foreach (var spellPair in root)
        {
            string spellName = spellPair.Key;
            JObject spellData = (JObject)spellPair.Value;

            spellDefinitions[spellName] = spellData;
        }
    }

    public Spell Build(string spellKey, SpellCaster owner)
    {
        if (!spellDefinitions.ContainsKey(spellKey))
        {
            Debug.LogError($"Spell '{spellKey}' not found in SpellBuilder.");
            return null;
        }

        JObject definition = spellDefinitions[spellKey];

        Spell spell;

        switch (spellKey)
        {
            case "arcane_blast":
                spell = new ArcaneBlast(owner);
                break;

            case "arcane_spray":
                spell = new ArcaneSpray(owner);
                break;

            case "turret_spell":
                spell = new TurretSpell(owner, TurretPrefab);
                break;

            case "arcane_ricochet":
                spell = new ArcaneRicochet(owner);
                break;
            default:
                spell = new Spell(owner);
                break;
        }

        spell.SetAttributes(definition);
        return spell;
    }

    public Spell ApplyModifiersToSpell(Spell baseSpell, List<string> modifierKeys)
    {
        Spell modifiedSpell = baseSpell;

        foreach (var modKey in modifierKeys)
        {
            if (!spellDefinitions.ContainsKey(modKey))
            {
                Debug.LogWarning($"Modifier '{modKey}' not found in SpellBuilder.");
                continue;
            }

            JObject modDef = spellDefinitions[modKey];

            switch (modKey)
            {
                case "doubler":
                    var doubler = new DoublerSpell(modifiedSpell);
                    doubler.SetAttributes(modDef);
                    modifiedSpell = doubler;
                    break;

                case "splitter":
                    var splitter = new SplitterSpell(modifiedSpell);
                    splitter.SetAttributes(modDef);
                    modifiedSpell = splitter;
                    break;

                case "burning":
                    var burning = new BurningModifierSpell(modifiedSpell);
                    burning.SetAttributes(modDef);
                    modifiedSpell = burning;
                    break;

                default:
                    var genericModifier = new ModifierSpell(modifiedSpell);
                    genericModifier.SetAttributes(modDef);
                    modifiedSpell = genericModifier;
                    break;
            }
        }

        return modifiedSpell;
    }

    public Spell CreateRandomSpell(SpellCaster owner)
    {
        var collectedModifiers = new List<string>();
        return PickRandom(owner, collectedModifiers);
    }

    private Spell PickRandom(SpellCaster owner, List<string> mods)
    {
        var keys = spellDefinitions.Keys.ToList();
        string pick = keys[Random.Range(0, keys.Count)];
        var def = spellDefinitions[pick];

        string type = def["type"] != null ? def["type"].ToString() : "";

        if (type == "modifier")
        {
            mods.Add(pick);
            return PickRandom(owner, mods);
        }
        else
        {
            Spell baseSpell = Build(pick, owner);
            if (mods.Count > 0)
                baseSpell = ApplyModifiersToSpell(baseSpell, mods);
            return baseSpell;
        }
    }

    public Spell CreateRandomReward(SpellCaster owner, out bool isModifier, out string modifierKey)
    {
        var keys = spellDefinitions.Keys.ToList();
        string pick = keys[Random.Range(0, keys.Count)];
        var def = spellDefinitions[pick];

        isModifier = def["type"] != null && def["type"].ToString() == "modifier";
        modifierKey = isModifier ? pick : null;

        if (isModifier)
        {
            Spell dummy = new Spell(owner);

            dummy.Icon = def["icon"] != null ? int.Parse(def["icon"].ToString()) : 0;
            dummy.Description = def["description"]?.ToString() ?? "No description";
            dummy.Name = def["name"]?.ToString() ?? "Modifier";

            // Set safe default values to prevent RPN crashes
            dummy.ManaCostExpr = "0";
            dummy.DamageExpr = "0";
            dummy.CooldownExpr = "0";

            return dummy;
        }
        else
        {
            return Build(pick, owner);
        }
    }

    
}


