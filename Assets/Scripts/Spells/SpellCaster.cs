using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellCaster
{
    public int mana;
    public int max_mana;
    public int mana_reg;
    public int power;
    public int spellCastIndex;
    public Hittable.Team team;
    public List<Spell> spells;
    public int maxSpellCount;

    public IEnumerator ManaRegeneration()
    {
        while (true)
        {
            mana += mana_reg;
            mana = Mathf.Min(mana, max_mana);
            yield return new WaitForSeconds(1);
        }
    }

    public SpellCaster(int mana, int mana_reg, Hittable.Team team)
    {
        this.mana = mana;
        this.max_mana = mana;
        this.mana_reg = mana_reg;
        this.power = 100;
        this.team = team;
        this.spells = new List<Spell>();
        this.spellCastIndex = 0;
        this.maxSpellCount = 4;

        AddSpell(CreateStartSpell());

        //spell = SpellBuilder.Instance.CreateRandomSpell(this);
        //spell = SpellBuilder.Instance.Build("turret_spell", this);
        //spell = SpellBuilder.Instance.ApplyModifiersToSpell(spell, new List<string> { "burning", "damage_amp", "doubler" });

        //DEBUG STATEMENTS
        // List<KeyValuePair<string, string>> def = spell.GetDefinitionList();
        // if (def != null && def.Count > 0)
        // {
        //     foreach (KeyValuePair<string, string> pair in def)
        //     {
        //         string key = pair.Key ?? "(null key)";
        //         string value = pair.Value ?? "(null value)";
        //         Debug.Log($"Spell: {key} | Definition: {value}");
        //     }
        // }
        // else
        // {
        //     Debug.Log("No spell definitions found.");
        // }
    }

    public void AddSpell(Spell spell)
    {
        if (this.spells.Count < this.maxSpellCount)
        {
            this.spells.Add(spell);
        }

        GameManager.Instance.player.GetComponent<PlayerController>().spellUIContainer.LoadUI(spells);
    }

    public void DropSpell(int index)
    {
        spells.RemoveAt(index);
        GameManager.Instance.player.GetComponent<PlayerController>().spellUIContainer.DeactiveAllUI();
        GameManager.Instance.player.GetComponent<PlayerController>().spellUIContainer.LoadUI(spells);
    }

    public Spell CreateStartSpell()
    {
        Spell spell = SpellBuilder.Instance.Build("arcane_bolt", this);
        spell = SpellBuilder.Instance.ApplyModifiersToSpell(spell, new List<string> { "mana_amp", "doubler", "burning" });
        return spell;
    }

    public int GetCurrentSpellAmount()
    {
        return spells.Count;
    }


    public IEnumerator Cast(Vector3 where, Vector3 target)
    {

        if (mana >= spells[spellCastIndex].GetManaCost() && spells[spellCastIndex].IsReady())
        {
            mana -= spells[spellCastIndex].GetManaCost();
            spells[spellCastIndex].SetLastcast(Time.time);
            yield return spells[spellCastIndex].Cast(where, target, team);
        }
        yield break;
    }

}


