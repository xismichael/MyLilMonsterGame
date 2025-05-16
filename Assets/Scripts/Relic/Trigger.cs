using System.Collections.Generic;

public class Trigger
{
    public string Type;       // e.g., "stand-still"
    public float Amount;      // optional threshold

    public bool ShouldActivate(Dictionary<string, object> parameters)
    {
        switch (Type)
        {
            case "stand-still":
                return parameters.ContainsKey("duration") && (float)parameters["duration"] >= Amount;
            case "take-damage":
                return true;
            case "on-kill":
                return true;
            default:
                return false;
        }
    }
}