using System.Collections.Generic;
using UnityEngine;

public class RelicEventManager : MonoBehaviour
{
    private RelicManager relicManager;

    void Awake()
    {
        relicManager = GetComponent<RelicManager>();
    }

    public void RegisterPlayer(PlayerController player)
    {
        player.OnStandStill += (duration) =>
        {
            Trigger("stand-still", player, new() { { "duration", duration } });
        };

        player.OnMoveEvent += (moveDir) =>
        {
            Trigger("move", player, new() { { "direction", moveDir } });
        };

        player.hp.OnHit += (dmg) =>
        {
            Trigger("take-damage", player, new() { { "damage", dmg } });
        };

        EventBus.Instance.OnAllEnemyDeath += () =>
        {
            Trigger("on-kill", player);
        };
    }

    public void Trigger(string triggerType, PlayerController player, Dictionary<string, object> parameters = null)
    {
        foreach (var relic in relicManager.GetOwnedRelics())
        {
            if (relic.Trigger.Type == triggerType)
            {
                relic.TryActivate(player, parameters);
            }
        }
    }
}