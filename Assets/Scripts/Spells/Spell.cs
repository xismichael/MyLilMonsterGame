using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System;

public class Spell
{
    public float lastCast;
    public SpellCaster owner;
    public Hittable.Team team;

    public string Name;
    public string FullName;
    public string Description;
    public int Icon;
    public string DamageExpr;
    public string ManaCostExpr;
    public string CooldownExpr;
    public List<KeyValuePair<string, string>> DefinitionList = new List<KeyValuePair<string, string>>();
    protected List<string> damageMultiplierExprs = new List<string>();
    protected List<string> damageAdderExprs = new List<string>();

    protected List<string> manaMultiplierExprs = new List<string>();
    protected List<string> manaAdderExprs = new List<string>();

    protected List<string> cooldownMultiplierExprs = new List<string>();
    protected List<string> cooldownAddExprs = new List<string>();

    protected List<string> speedMultiplierExprs = new List<string>();
    protected List<string> speedAdderExprs = new List<string>();

    protected List<string> lifetimeMultiplierExprs = new List<string>();
    protected List<string> lifetimeAdderExprs = new List<string>();

    public float damageAtTimeOfCast;
    public ProjectileData ProjectileData;

    public List<Action<Hittable, Vector3>> OnHitHandlers = new List<Action<Hittable, Vector3>>();

    public Spell(SpellCaster owner)
    {
        this.owner = owner;
    }

    public virtual void SetAttributes(JObject attributes)
    {
        Name = attributes["name"].ToString();
        FullName = attributes["name"].ToString();
        Description = attributes["description"].ToString();
        Icon = attributes["icon"].ToObject<int>();

        DamageExpr = attributes["damage"]["amount"].ToString();
        ManaCostExpr = attributes["mana_cost"].ToString();
        CooldownExpr = attributes["cooldown"].ToString();

        ProjectileData = new ProjectileData
        {
            Trajectory = attributes["projectile"]["trajectory"].ToString(),
            SpeedExpr = attributes["projectile"]["speed"].ToString(),
            Sprite = attributes["projectile"]["sprite"].ToObject<int>()
        };

        RegisterDefinition(Name, attributes["description"].ToString());

        if (attributes["projectile"]["lifetime"] != null)
        {
            ProjectileData.LifetimeExpr = attributes["projectile"]["lifetime"].ToString();
        }
       
    }

    public virtual void RegisterDefinition(string spellName, string definition)
    {
        int index = DefinitionList.FindIndex(pair => pair.Key == spellName);
        if (index < 0)
        {
            DefinitionList.Add(new KeyValuePair<string, string>(spellName, definition));
        }
    }
    public virtual List<KeyValuePair<string, string>> GetDefinitionList()
    {
        return DefinitionList;
    }

    public Dictionary<string, float> GetRPNVariables()
    {
        return new Dictionary<string, float>
        {
            { "power", owner.power },
            { "wave", EnemySpawner.CurrentWaveNumber }
        };
    }

    public virtual string GetBaseName()
    {
        return Name;
    }

    public virtual string GetFullName()
    {
        return FullName;
    }

    public virtual void AddFullName(string modName)
    {
        FullName = modName + " " + FullName;
    }


    public virtual float GetDamage()
    {
        float result = RPNEvaluator.Evaluate(DamageExpr, GetRPNVariables());

        foreach (var expr in damageMultiplierExprs)
            result *= RPNEvaluator.Evaluate(expr, GetRPNVariables());

        foreach (var expr in damageAdderExprs)
            result += RPNEvaluator.Evaluate(expr, GetRPNVariables());

        return result;
    }

    public virtual int GetManaCost()
    {
        float result = 0f;

        try
        {
            // Safely evaluate base cost
            if (!string.IsNullOrEmpty(ManaCostExpr))
                result = RPNEvaluator.Evaluate(ManaCostExpr, GetRPNVariables());

            // Safely evaluate adders
            if (manaAdderExprs != null)
            {
                foreach (var expr in manaAdderExprs)
                {
                    if (!string.IsNullOrEmpty(expr))
                        result += RPNEvaluator.Evaluate(expr, GetRPNVariables());
                }
            }

            // Safely evaluate multipliers
            if (manaMultiplierExprs != null)
            {
                foreach (var expr in manaMultiplierExprs)
                {
                    if (!string.IsNullOrEmpty(expr))
                        result *= RPNEvaluator.Evaluate(expr, GetRPNVariables());
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[Spell.GetManaCost] Failed to evaluate mana cost");
            result = 0f;
        }

        return Mathf.RoundToInt(result);
    }

    public virtual float GetCooldown()
    {
        float result = RPNEvaluator.Evaluate(CooldownExpr, GetRPNVariables());

        foreach (var expr in cooldownMultiplierExprs)
            result *= RPNEvaluator.Evaluate(expr, GetRPNVariables());
        foreach (var expr in cooldownAddExprs)
            result += RPNEvaluator.Evaluate(expr, GetRPNVariables());
        return result;
    }

    public virtual float GetProjectileSpeed()
    {
        float result = RPNEvaluator.Evaluate(ProjectileData.SpeedExpr, GetRPNVariables());

        foreach (var expr in speedAdderExprs)
            result += RPNEvaluator.Evaluate(expr, GetRPNVariables());

        foreach (var expr in speedMultiplierExprs)
            result *= RPNEvaluator.Evaluate(expr, GetRPNVariables());

        return result;
    }

    public virtual float GetProjectileLifetime()
    {
        float result = RPNEvaluator.Evaluate(ProjectileData.LifetimeExpr, GetRPNVariables());

        foreach (var expr in lifetimeAdderExprs)
            result += RPNEvaluator.Evaluate(expr, GetRPNVariables());

        foreach (var expr in lifetimeMultiplierExprs)
            result *= RPNEvaluator.Evaluate(expr, GetRPNVariables());

        return result;
    }

    public virtual int GetIcon()
    {
        return Icon;
    }

    public virtual bool IsReady()
    {
        return (lastCast + GetCooldown() < Time.time);
    }

    public virtual void AddDamageMultiplier(string expr) => damageMultiplierExprs.Add(expr);
    public virtual void AddDamageAdder(string expr) => damageAdderExprs.Add(expr);

    public virtual void AddManaMultiplier(string expr) => manaMultiplierExprs.Add(expr);
    public virtual void AddManaAdder(string expr) => manaAdderExprs.Add(expr);

    public virtual void AddCooldownMultiplier(string expr) => cooldownMultiplierExprs.Add(expr);
    public virtual void AddCooldownAdder(string expr) => cooldownAddExprs.Add(expr);

    public virtual void AddSpeedMultiplier(string expr) => speedMultiplierExprs.Add(expr);
    public virtual void AddSpeedAdder(string expr) => speedAdderExprs.Add(expr);

    public virtual void AddLifetimeMultiplier(string expr) => lifetimeMultiplierExprs.Add(expr);
    public virtual void AddLifetimeAdder(string expr) => lifetimeAdderExprs.Add(expr);

    public virtual void SetPierceCount(string expr)
    {
        ProjectileData.PierceCount = expr;
    }
    public virtual void SetProjectile(string expr)
    {
        ProjectileData.Trajectory = expr;
    }

    public virtual void SetLastcast(float time)
    {
        lastCast = time;
    }
    public virtual float GetLastcast()
    {
        return lastCast;
    }

    public virtual Action<Hittable, Vector3> GetOnHitHandler()
    {
        return this.OnHit;
    }
    public virtual void AddOnHitHandler(Action<Hittable, Vector3> onHitHandler) => OnHitHandlers.Add(onHitHandler);

    public virtual IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        this.team = team;

        float speed = GetProjectileSpeed();
        damageAtTimeOfCast = GetDamage();

        if (!string.IsNullOrEmpty(ProjectileData.LifetimeExpr))
        {
            float lifetime = GetProjectileLifetime();
            GameManager.Instance.projectileManager.CreateProjectile(
                ProjectileData.Sprite,
                ProjectileData.Trajectory,
                where,
                target - where,
                speed,
                this.GetOnHitHandler(),
                lifetime
            );
        }
        else if (!string.IsNullOrEmpty(ProjectileData.PierceCount))
        {
            int count = 0;
            int.TryParse(ProjectileData.PierceCount, out count);
            GameManager.Instance.projectileManager.CreateProjectile(
                ProjectileData.Sprite,
                ProjectileData.Trajectory,
                where,
                target - where,
                speed,
                this.GetOnHitHandler(),
                count
            );
        }
        else
        {
            GameManager.Instance.projectileManager.CreateProjectile(
                ProjectileData.Sprite,
                ProjectileData.Trajectory,
                where,
                target - where,
                speed,
                this.GetOnHitHandler()
            );
        }
        yield return new WaitForEndOfFrame();
    }

    public virtual void OnHit(Hittable other, Vector3 impact)
    {
        if (other.team != team)
        {
            other.Damage(new Damage(Mathf.RoundToInt(damageAtTimeOfCast), Damage.Type.ARCANE));
            foreach (var handler in OnHitHandlers)
            {
                handler.Invoke(other, impact);
            }
        }
    }
}
