using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

public class EnemyDatabase : MonoBehaviour
{
    private static EnemyDatabase theInstance;
    public static EnemyDatabase Instance
    {
        get
        {
            return theInstance;
        }
    }

    private Dictionary<string, Enemy> enemyDict;

    //load database on runtime
    void Awake()
    {
        if (theInstance == null)
        {
            theInstance = this;
            LoadEnemies();
        }

    }

    //populate the dictionary
    private void LoadEnemies()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("enemies");
        List<Enemy> enemyList = JsonConvert.DeserializeObject<List<Enemy>>(jsonFile.text);
        enemyDict = enemyList.ToDictionary(e => e.name);
    }

    //for getting individual enemy data
    public Enemy GetEnemyData(string name)
    {
        if (enemyDict.ContainsKey(name)) return enemyDict[name];
        return null;
    }



}