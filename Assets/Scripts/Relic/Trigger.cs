using System.Collections.Generic;
using UnityEngine;

public class Trigger
{
    public string Type;
    public float Amount = 0f;
    public float Percentage = -1f;
    public string Condition;

    public string Mode = "repeat";
    public float Interval = 0f;

    private float lastActivationTime = -Mathf.Infinity;
    private bool hasActivated = false;

    public bool ShouldActivate(Dictionary<string, object> parameters)
    {
        switch (Type)
        {
            case "stand-still":
                return CheckStandStill(parameters);
            case "take-damage":
                return CheckTakeDamage(parameters);
            case "on-kill":
                return CheckTimedOrOnce();
            case "health-percentage":
                return CheckHealthPercentage(parameters);
            default:
                return false;
        }
    }

    private bool CheckStandStill(Dictionary<string, object> parameters)
    {
        if (!parameters.ContainsKey("duration")) return false;
        float duration = (float)parameters["duration"];
        return EvaluateTrigger(duration >= Amount);
    }

    private bool CheckTakeDamage(Dictionary<string, object> parameters)
    {
        if (parameters.TryGetValue("damage", out object dmgObj) && dmgObj is Damage dmg)
        {
            return EvaluateTrigger(dmg.amount >= Amount);
        }
        return false;
    }

    private bool CheckHealthPercentage(Dictionary<string, object> parameters)
    {
        if (!parameters.ContainsKey("playerHealth") || parameters["playerHealth"] is not Hittable playerHealth)
            return false;

        float current = playerHealth.hp;
        float max = playerHealth.max_hp;
        float percent = current / max;

        bool conditionMet = Condition switch
        {
            "below" => percent < Percentage,
            "above" => percent > Percentage,
            "equal" => Mathf.Approximately(percent, Percentage),
            _ => false
        };

        return EvaluateTrigger(conditionMet);
    }

    private bool CheckTimedOrOnce()
    {
        return EvaluateTrigger(true); // Always true, applies mode timing
    }

    private bool EvaluateTrigger(bool conditionMet)
    {
        if (!conditionMet)
        {
            // ❗ Reset when condition becomes false
            hasActivated = false;
            return false;
        }

        if (Mode == "once")
        {
            if (!hasActivated)
            {
                hasActivated = true;
                return true;
            }
        }
        else // repeat
        {
            if (Time.time >= lastActivationTime + Interval)
            {
                lastActivationTime = Time.time;
                return true;
            }
        }

        return false;
    }

    public void Reset()
    {
        hasActivated = false;
        lastActivationTime = -Mathf.Infinity;
    }
}
