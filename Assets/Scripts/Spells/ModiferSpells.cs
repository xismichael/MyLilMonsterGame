using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

public class DoublerSpell : ModifierSpell
{
    private string delayExpr;

    public DoublerSpell(Spell innerSpell) : base(innerSpell) { }

    public override void SetAttributes(JObject attributes)
    {
        base.SetAttributes(attributes);
        delayExpr = attributes["delay"].ToString();
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        float delay = RPNEvaluator.Evaluate(delayExpr, GetRPNVariables());

        yield return innerSpell.Cast(where, target, team);

        yield return new WaitForSeconds(delay);

        Vector3 updatedPos = GameManager.Instance.player.transform.position;
        yield return innerSpell.Cast(updatedPos, target, team);
    }
}

public class SplitterSpell : ModifierSpell
{
    private string angleExpr;

    public SplitterSpell(Spell innerSpell) : base(innerSpell) { }

    public override void SetAttributes(JObject attributes)
    {
        base.SetAttributes(attributes);
        angleExpr = attributes["angle"].ToString();
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        float angle = RPNEvaluator.Evaluate(angleExpr, GetRPNVariables());
        Vector3 forward = (target - where).normalized;

        // Rotate +angle
        Vector3 dirPlus = Quaternion.Euler(0, 0, angle) * forward;
        CoroutineManager.Instance.Run(innerSpell.Cast(where, where + dirPlus, team));
        // Rotate -angle
        Vector3 dirMinus = Quaternion.Euler(0, 0, -angle) * forward;
        CoroutineManager.Instance.Run(innerSpell.Cast(where, where + dirMinus, team));
        yield return new WaitForEndOfFrame();
    }
}

public class BurningModifierSpell : ModifierSpell
{
    private string tickCountExpr;
    private string damagePerTickExpr;

    public BurningModifierSpell(Spell innerSpell) : base(innerSpell) { }

    public override void SetAttributes(JObject attributes)
    {
        base.SetAttributes(attributes);
        tickCountExpr = attributes["ticks"].ToString();
        damagePerTickExpr = attributes["damage_per_tick"].ToString();

        // Add the burn effect to the inner spell's OnHit handlers
        innerSpell.AddOnHitHandler(ApplyBurnEffect);
    }

    private void ApplyBurnEffect(Hittable target, Vector3 impact)
    {
        int ticks = Mathf.RoundToInt(RPNEvaluator.Evaluate(tickCountExpr, GetRPNVariables()));
        float damagePerTick = RPNEvaluator.Evaluate(damagePerTickExpr, GetRPNVariables());

        CoroutineManager.Instance.Run(BurnCoroutine(target, ticks, damagePerTick));
    }

    private IEnumerator BurnCoroutine(Hittable target, int ticks, float damagePerTick)
    {
        for (int i = 0; i < ticks; i++)
        {
            if (target != null)
            {
                target.Damage(new Damage(Mathf.RoundToInt(damagePerTick), Damage.Type.FIRE));
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}

