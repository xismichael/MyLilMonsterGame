using System.Collections.Generic;
using UnityEngine;

public class Relic
{
    public string Name;
    public int SpriteIndex;
    public Trigger Trigger;
    public Effect Effect;

    private bool hasTriggered = false;

    public void TryActivate(PlayerController player, Dictionary<string, object> parameters)
    {
        if (!hasTriggered && Trigger.ShouldActivate(parameters))
        {
            hasTriggered = true;
            Effect.Apply(player, this);
        }
    }

    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}