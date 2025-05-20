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

    void Update()
    {
        if (GameManager.Instance.state == GameManager.GameState.PREGAME ||
            GameManager.Instance.state == GameManager.GameState.WAVEEND ||
            GameManager.Instance.state == GameManager.GameState.GAMEOVER)
        {
            ResetAllTrigger();
        }
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

            JObject triggerObj = (JObject)relicObj["trigger"];
            JObject effectObj = (JObject)relicObj["effect"];

            Relic relic = new Relic
            {
                Name = name,
                SpriteIndex = (int)relicObj["sprite"],
                Trigger = new Trigger
                {
                    Type = triggerObj["type"].ToString(),
                    Amount = triggerObj["amount"] != null ? float.Parse(triggerObj["amount"].ToString()) : 0f,
                    Mode = triggerObj["mode"]?.ToString().ToLower() ?? "repeat",
                    Interval = triggerObj["interval"] != null ? float.Parse(triggerObj["interval"].ToString()) : 0f,
                    Percentage = triggerObj["percentage"] != null ? float.Parse(triggerObj["percentage"].ToString()) : -1f,
                    Condition = triggerObj["condition"]?.ToString().ToLower()
                },
                Effect = new Effect
                {
                    Type = effectObj["type"].ToString(),
                    AmountExpr = effectObj["amount"]?.ToString(),
                    Until = effectObj["until"]?.ToString(),
                    Percentage = effectObj["percentage"] != null ? float.Parse(effectObj["percentage"].ToString()) : -1f,
                    Condition = effectObj["condition"]?.ToString().ToLower()
                }
            };

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
        player.OnHealthChange += (hittable) =>
        {
            Trigger("health-percentage", player, new() { { "playerHealth", hittable } });
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
        player.OnHealthChange -= (hittable) =>
        {
            Trigger("health-percentage", player, new() { { "playerHealth", hittable } });
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

    public void ResetAllTrigger()
    {
        foreach (Relic relic in ownedRelics)
        {
            relic.Trigger.Reset();
        }
    }
}
