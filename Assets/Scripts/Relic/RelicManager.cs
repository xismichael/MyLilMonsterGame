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

    private void LoadRelicsFromJson()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("relics");
        JArray relicArray = JArray.Parse(jsonFile.text);

        foreach (var relicObj in relicArray)
        {
            Relic relic = new Relic
            {
                Name = relicObj["name"].ToString(),
                SpriteIndex = (int)relicObj["sprite"],
                Trigger = new Trigger
                {
                    Type = relicObj["trigger"]["type"].ToString(),
                    Amount = relicObj["trigger"]["amount"] != null ? float.Parse(relicObj["trigger"]["amount"].ToString()) : 0f
                },
                Effect = new Effect
                {
                    Type = relicObj["effect"]["type"].ToString(),
                    AmountExpr = relicObj["effect"]["amount"]?.ToString(),
                    Until = relicObj["effect"]["until"]?.ToString()
                }
            };

            allRelics[relic.Name] = relic;
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

    public List<Relic> GetOwnedRelics()
    {
        return ownedRelics;
    }
}
