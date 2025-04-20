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
