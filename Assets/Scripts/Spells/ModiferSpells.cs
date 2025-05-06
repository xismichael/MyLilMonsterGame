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
        lastCast = Time.time;

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
        lastCast = Time.time;
        float angle = RPNEvaluator.Evaluate(angleExpr, GetRPNVariables());
        Vector3 forward = (target - where).normalized;

        // Rotate +angle
        Vector3 dirPlus = Quaternion.Euler(0, 0, angle) * forward;
        yield return innerSpell.Cast(where, where + dirPlus, team);

        // Rotate -angle
        Vector3 dirMinus = Quaternion.Euler(0, 0, -angle) * forward;
        yield return innerSpell.Cast(where, where + dirMinus, team);
    }
}