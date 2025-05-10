using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

public class LevelDatabase : MonoBehaviour
{
    private static LevelDatabase instance;
    public static LevelDatabase Instance
    {
        get { return instance; }
    }

    private List<Level> levels;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLevels();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadLevels()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("levels");
        levels = JsonConvert.DeserializeObject<List<Level>>(jsonFile.text);

        foreach (var level in levels)
        {
            foreach (var spawn in level.spawns)
            {
                if (spawn.sequence == null || spawn.sequence.Count == 0) spawn.sequence = new List<int> { 1 };
                if (string.IsNullOrEmpty(spawn.delay)) spawn.delay = "2";
                if (string.IsNullOrEmpty(spawn.hp)) spawn.hp = "base";
                if (string.IsNullOrEmpty(spawn.damage)) spawn.damage = "base";
                if (string.IsNullOrEmpty(spawn.speed)) spawn.speed = "base";
                if (string.IsNullOrEmpty(spawn.location)) spawn.speed = "random";
            }
        }
    }


    public Level GetLevel(string name)
    {
        return levels.Find(level => level.name == name);
    }

    public List<Level> GetAllLevels()
    {
        return levels;
    }
}
