using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System;

public class ArcaneBlast : Spell
{
    public string BoltCountExpr;
    public string SecondaryDamageExpr;
    public ProjectileData SecondaryProjectile;

    public ArcaneBlast(SpellCaster owner) : base(owner) { }

    public override void SetAttributes(JObject attributes)
    {
        base.SetAttributes(attributes);

        BoltCountExpr = attributes["N"].ToString();
        SecondaryDamageExpr = attributes["secondary_damage"].ToString();

        SecondaryProjectile = new ProjectileData
        {
            Trajectory = attributes["secondary_projectile"]["trajectory"].ToString(),
            SpeedExpr = attributes["secondary_projectile"]["speed"].ToString(),
            Sprite = attributes["secondary_projectile"]["sprite"].ToObject<int>(),
            LifetimeExpr = attributes["secondary_projectile"]["lifetime"].ToString()
        };
    }

    public override Action<Hittable, Vector3> GetOnHitHandler()
    {
        return ArcaneBlastOnHit;
    }

    void ArcaneBlastOnHit(Hittable other, Vector3 impact)
    {
        if (other.team != team)
        {
            other.Damage(new Damage(Mathf.RoundToInt(GetDamage()), Damage.Type.ARCANE));

            int secondaryCount = Mathf.RoundToInt(RPNEvaluator.Evaluate(BoltCountExpr, GetRPNVariables()));
            float secondarySpeed = RPNEvaluator.Evaluate(SecondaryProjectile.SpeedExpr, GetRPNVariables());
            float secondaryLifetime = RPNEvaluator.Evaluate(SecondaryProjectile.LifetimeExpr, GetRPNVariables());
            float secondaryDamage = RPNEvaluator.Evaluate(SecondaryDamageExpr, GetRPNVariables());

            for (int i = 0; i < secondaryCount; i++)
            {
                Vector3 randomDirection = UnityEngine.Random.insideUnitSphere.normalized;

                GameManager.Instance.projectileManager.CreateProjectile(
                    SecondaryProjectile.Sprite,
                    SecondaryProjectile.Trajectory,
                    impact,
                    randomDirection,
                    secondarySpeed,
                    (secOther, secImpact) =>
                    {
                        if (secOther.team != team)
                        {
                            secOther.Damage(new Damage(Mathf.RoundToInt(secondaryDamage), Damage.Type.ARCANE));
                        }
                    },
                    secondaryLifetime
                );
            }
        }
    }
}

public class ArcaneSpray : Spell
{
    public string ShotCountExpr;
    public string SprayAngleExpr;

    public ArcaneSpray(SpellCaster owner) : base(owner) { }

    public override void SetAttributes(JObject attributes)
    {
        base.SetAttributes(attributes);

        ShotCountExpr = attributes["N"].ToString();
        SprayAngleExpr = attributes["spray"].ToString();

        if (attributes["projectile"]["lifetime"] != null)
        {
            ProjectileData.LifetimeExpr = attributes["projectile"]["lifetime"].ToString();
        }
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        this.team = team;

        Vector3 forward = (target - where).normalized;
        float speed = RPNEvaluator.Evaluate(ProjectileData.SpeedExpr, GetRPNVariables());
        float lifetime = string.IsNullOrEmpty(ProjectileData.LifetimeExpr) ? 1f : RPNEvaluator.Evaluate(ProjectileData.LifetimeExpr, GetRPNVariables());

        int shotCount = Mathf.RoundToInt(RPNEvaluator.Evaluate(ShotCountExpr, GetRPNVariables()));
        float sprayAngle = RPNEvaluator.Evaluate(SprayAngleExpr, GetRPNVariables());

        for (int i = 0; i < shotCount; i++)
        {
            float angleOffset = UnityEngine.Random.Range(-sprayAngle, sprayAngle);
            Vector3 rotatedDirection = Quaternion.Euler(0, 0, angleOffset) * forward;

            GameManager.Instance.projectileManager.CreateProjectile(
                ProjectileData.Sprite,
                ProjectileData.Trajectory,
                where,
                rotatedDirection,
                speed,
                OnHit,
                lifetime
            );
        }

        lastCast = Time.time;
        yield return new WaitForEndOfFrame();
    }
}
