using System.Collections.Generic;
using UnityEngine;

public class Trigger
{
    public string Type;       // e.g., "stand-still"
    public float Amount = 0;      // optional threshold

    public string Mode = "repeat";
    public float Interval = 0f;

    private float lastActivationTime = -Mathf.Infinity;
    private bool hasActivated = false;

    public bool ShouldActivate(Dictionary<string, object> parameters)
    {
        switch (Type)
        {
            case "stand-still":
                return StandStill(parameters);
            case "take-damage":
                return TakeDamage(parameters);
            case "on-kill":
                return true;
            default:
                return false;
        }
    }

    private bool StandStill(Dictionary<string, object> parameters)
    {
        if (!parameters.ContainsKey("duration")) return false;
        float duration = (float)parameters["duration"];

        if (Mode == "once")
        {
            if (duration >= Amount && !hasActivated)
            {
                hasActivated = true;
                return true;
            }
            else if (duration < Amount)
            {
                hasActivated = false;
            }
        }
        else // repeat
        {
            if (duration >= Amount && Time.time >= lastActivationTime + Interval)
            {
                lastActivationTime = Time.time;
                return true;
            }
        }
        return false;
    }

    private bool TakeDamage(Dictionary<string, object> parameters)
    {
        if (parameters.ContainsKey("damage") && parameters["damage"] is Damage dmg)
        {
            return dmg.amount >= Amount;
        }
        return false;
    }

}