using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class RelicManager : MonoBehaviour
{
    private Dictionary<string, Relic> allRelics = new();
    private List<Relic> ownedRelics = new();

    void Awake()
    {
        LoadRelicsFromJson();
    }

    public void LoadRelicsFromJson()
    {
        allRelics = new Dictionary<string, Relic>();

        TextAsset jsonFile = Resources.Load<TextAsset>("relics");
        if (jsonFile == null)
        {
            Debug.LogError("relics.json not found in Resources!");
            return;
        }

        JArray relicArray = JArray.Parse(jsonFile.text);

        foreach (var relicToken in relicArray)
        {
            JObject relicObj = (JObject)relicToken;
            string name = relicObj["name"].ToString();

            Relic relic = new Relic
            {
                Name = name,
                SpriteIndex = (int)relicObj["sprite"],
                Trigger = new Trigger
                {
                    Type = relicObj["trigger"]["type"].ToString(),
                    Amount = relicObj["trigger"]["amount"] != null ? float.Parse(relicObj["trigger"]["amount"].ToString()) : 0f,
                    Mode = relicObj["trigger"]["mode"]?.ToString().ToLower() ?? "repeat",
                    Interval = relicObj["trigger"]["interval"] != null ? float.Parse(relicObj["trigger"]["interval"].ToString()) : 0f
                },
                Effect = new Effect
                {
                    Type = relicObj["effect"]["type"].ToString(),
                    AmountExpr = relicObj["effect"]["amount"]?.ToString(),
                    Until = relicObj["effect"]["until"]?.ToString()
                }
            }
        ;

            allRelics[name] = relic;
        }
    }

    public void AddRelic(string relicName)
    {
        if (!allRelics.ContainsKey(relicName))
        {
            Debug.LogWarning("Relic not found: " + relicName);
            return;
        }

        ownedRelics.Add(allRelics[relicName]);
    }

    public void RemoveRelic(string relicName)
    {
        ownedRelics.RemoveAll(r => r.Name == relicName);
    }

    public void RemoveAllRelics(PlayerController player)
    {
        foreach (var relic in ownedRelics)
        {
            relic.Effect?.Revert(player);   // revert any active effects
            relic.ResetTrigger();           // reset so it can be reused later
        }

        ownedRelics.Clear();
    }


    public List<Relic> GetOwnedRelics()
    {
        return ownedRelics;
    }

    public void Register(PlayerController player)
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

    public void UnRegister(PlayerController player)
    {
        player.OnStandStill -= (duration) =>
        {
            Trigger("stand-still", player, new() { { "duration", duration } });
        };
        player.OnMoveEvent -= (moveDir) =>
        {
            Trigger("move", player, new() { { "direction", moveDir } });
        };
        player.hp.OnHit -= (dmg) =>
        {
            Trigger("take-damage", player, new() { { "damage", dmg } });
        };
        EventBus.Instance.OnAllEnemyDeath -= () =>
        {
            Trigger("on-kill", player);
        };
    }

    public void Trigger(string triggerType, PlayerController player, Dictionary<string, object> parameters = null)
    {
        foreach (var relic in GetOwnedRelics())
        {
            if (relic.Trigger.Type == triggerType)
            {
                relic.TryActivate(player, parameters);
            }
        }
    }
}
