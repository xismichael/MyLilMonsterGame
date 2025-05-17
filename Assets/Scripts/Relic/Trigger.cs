using System.Collections.Generic;

public class Trigger
{
    public string Type;       // e.g., "stand-still"
    public float Amount = 0;      // optional threshold

    public bool ShouldActivate(Dictionary<string, object> parameters)
    {
        switch (Type)
        {
            case "stand-still":
                return StandStill(parameters);
            case "take-damage":
                return true;
            case "on-kill":
                return true;
            default:
                return false;
        }
    }

    private bool StandStill(Dictionary<string, object> parameters)
    {
        if (parameters.ContainsKey("duration"))
        {
            return (float)parameters["duration"] >= Amount;
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