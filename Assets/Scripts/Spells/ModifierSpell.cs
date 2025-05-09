using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System;

public class ModifierSpell : Spell
{
    protected Spell innerSpell;


    public ModifierSpell(Spell innerSpell) : base(innerSpell.owner)
    {
        this.innerSpell = innerSpell;
    }

    public override void SetAttributes(JObject attributes)
    {
        if (attributes["damage_multiplier"] != null)
        {
            AddDamageMultiplier(attributes["damage_multiplier"].ToString());
        }

        if (attributes["damage_adder"] != null)
        {
            AddDamageAdder(attributes["damage_adder"].ToString());
        }

        if (attributes["mana_multiplier"] != null)
        {
            AddManaMultiplier(attributes["mana_multiplier"].ToString());
        }

        if (attributes["mana_adder"] != null)
        {
            AddManaAdder(attributes["mana_adder"].ToString());
        }

        if (attributes["cooldown_multiplier"] != null)
        {
            AddCooldownMultiplier(attributes["cooldown_multiplier"].ToString());
        }

        if (attributes["cooldown_adder"] != null)
        {
            AddCooldownAdder(attributes["cooldown_multiplier"].ToString());
        }

        if (attributes["speed_multiplier"] != null)
        {
            AddSpeedMultiplier(attributes["speed_multiplier"].ToString());
        }

        if (attributes["speed_adder"] != null)
        {
            AddSpeedAdder(attributes["speed_adder"].ToString());
        }

        if (attributes["lifetime_multiplier"] != null)
        {
            AddLifetimeMultiplier(attributes["lifetime_multiplier"].ToString());
        }

        if (attributes["lifetime_adder"] != null)
        {
            AddLifetimeAdder(attributes["lifetime_adder"].ToString());
        }

        if (attributes["projectile_trajectory"] != null)
            SetProjectile(attributes["projectile_trajectory"].ToString());

        RegisterDefinition(attributes["name"].ToString(), attributes["description"].ToString());
        AddFullName(attributes["name"].ToString());

    }

    public override string GetBaseName()
    {
        return innerSpell.GetBaseName();
    }

    public override string GetFullName()
    {
        return innerSpell.GetFullName();
    }

    public override void AddFullName(string modName)
    {
        innerSpell.AddFullName(modName);
    }

    public override void RegisterDefinition(string spellName, string definition)
    {
        innerSpell.RegisterDefinition(spellName, definition);
    }
    public override List<KeyValuePair<string, string>> GetDefinitionList()
    {
        return innerSpell.GetDefinitionList();
    }
    public override void AddDamageMultiplier(string expr)
    {
        innerSpell.AddDamageMultiplier(expr);
    }
    public override void AddDamageAdder(string expr)
    {
        innerSpell.AddDamageAdder(expr);
    }

    public override void AddManaMultiplier(string expr)
    {
        innerSpell.AddManaMultiplier(expr);
    }
    public override void AddManaAdder(string expr)
    {
        innerSpell.AddManaAdder(expr);
    }

    public override void AddCooldownMultiplier(string expr)
    {
        innerSpell.AddCooldownMultiplier(expr);
    }
    public override void AddCooldownAdder(string expr)
    {
        innerSpell.AddCooldownAdder(expr);
    }

    public override void AddSpeedMultiplier(string expr)
    {
        innerSpell.AddSpeedMultiplier(expr);
    }
    public override void AddSpeedAdder(string expr)
    {
        innerSpell.AddSpeedAdder(expr);
    }

    public override void AddLifetimeMultiplier(string expr)
    {
        innerSpell.AddLifetimeMultiplier(expr);
    }
    public override void AddLifetimeAdder(string expr)
    {
        innerSpell.AddLifetimeAdder(expr);
    }

    public override void SetProjectile(string expr)
    {
        innerSpell.SetProjectile(expr);
    }

    public override void AddOnHitHandler(Action<Hittable, Vector3> onHitHandler)
    {
        innerSpell.AddOnHitHandler(onHitHandler);
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        // Use the inner spell's casting behavior
        return innerSpell.Cast(where, target, team);
    }

    public override Action<Hittable, Vector3> GetOnHitHandler()
    {
        return innerSpell.GetOnHitHandler();
    }

    public override float GetDamage()
    {
        return innerSpell.GetDamage();
    }
    public override int GetManaCost()
    {
        return innerSpell.GetManaCost();
    }
    public override float GetCooldown()
    {
        return innerSpell.GetCooldown();
    }
    public override float GetProjectileSpeed()
    {
        return innerSpell.GetProjectileSpeed();
    }
    public override float GetProjectileLifetime()
    {
        return innerSpell.GetProjectileLifetime();
    }

    public override void SetLastcast(float time)
    {
        innerSpell.SetLastcast(time);
    }
    public override float GetLastcast()
    {
        return innerSpell.GetLastcast();
    }

    public override int GetIcon()
    {
        return innerSpell.GetIcon();
    }

    public override bool IsReady()
    {
        return innerSpell.IsReady();
    }
}
