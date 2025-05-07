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
    public string Description;
    public int Icon;

    public string DamageExpr;
    public string ManaCostExpr;
    public string CooldownExpr;

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

    public ProjectileData ProjectileData;

    public Spell(SpellCaster owner)
    {
        this.owner = owner;
    }

    public virtual void SetAttributes(JObject attributes)
    {
        Name = attributes["name"].ToString();
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

        if (attributes["projectile"]["lifetime"] != null)
        {
            ProjectileData.LifetimeExpr = attributes["projectile"]["lifetime"].ToString();
        }
    }

    public Dictionary<string, float> GetRPNVariables()
    {
        return new Dictionary<string, float>
        {
            { "power", owner.power },
            { "wave", EnemySpawner.CurrentWaveNumber }
        };
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
        float result = RPNEvaluator.Evaluate(ManaCostExpr, GetRPNVariables());


        foreach (var expr in manaAdderExprs)
            result += RPNEvaluator.Evaluate(expr, GetRPNVariables());

        foreach (var expr in manaMultiplierExprs)
            result *= RPNEvaluator.Evaluate(expr, GetRPNVariables());

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

    public virtual IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        this.team = team;

        float speed = GetProjectileSpeed();

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
            other.Damage(new Damage(Mathf.RoundToInt(this.GetDamage()), Damage.Type.ARCANE));
        }
    }
}
