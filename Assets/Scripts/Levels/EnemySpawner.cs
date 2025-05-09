using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    public Image level_selector;
    public GameObject button;
    public GameObject enemy;
    public SpawnPoint[] SpawnPoints;
    public TMP_Text nextWaveButtonText;

    public int currentWaveEnemiesKilled;
    public int TotalEnemiesKilled;
    public int currentWaveDamageTaken;
    public int TotalDamageTaken;

    public static int CurrentWaveNumber = 1;
    private string currentLevelName;

    public static EnemySpawner Instance { get; private set; }

    void Start()
    {
        Instance = this;
        makeRestartScreen();
    }

    public void restartScreen()
    {
        level_selector.gameObject.SetActive(true);
    }

    public void makeRestartScreen()
    {
        float yStart = 130;
        float ySpacing = -70;
        int index = 0;

        List<Level> allLevels = LevelDatabase.Instance.GetAllLevels();

        foreach (Level level in allLevels)
        {
            GameObject selector = Instantiate(button, level_selector.transform);
            selector.transform.localPosition = new Vector3(0, yStart + index * ySpacing);
            selector.GetComponent<MenuSelectorController>().spawner = this;
            selector.GetComponent<MenuSelectorController>().SetLevel(level.name);
            index++;
        }
    }

    public void StartLevel(string levelname)
    {
        level_selector.gameObject.SetActive(false);

        TotalEnemiesKilled = 0;
        TotalDamageTaken = 0;

        GameManager.Instance.player.GetComponent<PlayerController>().StartLevel();
        currentLevelName = levelname;
        CurrentWaveNumber = 1;
        StartCoroutine(SpawnWave(levelname));
    }


    public void SpawnNextWave()
    {
        currentWaveEnemiesKilled = 0;
        currentWaveDamageTaken = 0;
        StartCoroutine(SpawnWave(currentLevelName));
        CurrentWaveNumber++;
    }

    IEnumerator SpawnWave(string levelname)
    {
        GameManager.Instance.state = GameManager.GameState.COUNTDOWN;
        GameManager.Instance.countdown = 3;

        for (int i = 3; i > 0; i--)
        {
            yield return new WaitForSeconds(1);
            GameManager.Instance.countdown--;
        }

        GameManager.Instance.state = GameManager.GameState.INWAVE;

        Level level = LevelDatabase.Instance.GetLevel(levelname);
        List<Coroutine> activeSpawns = new List<Coroutine>();

        foreach (Spawn spawn in level.spawns)
        {
            Coroutine activeSpawn = StartCoroutine(SpawnEnemies(spawn));
            activeSpawns.Add(activeSpawn);
        }

        foreach (Coroutine activeSpawn in activeSpawns)
        {
            yield return activeSpawn;
        }

        yield return new WaitWhile(() => GameManager.Instance.enemy_count > 0);

        if (CurrentWaveNumber >= level.waves) { GameManager.Instance.state = GameManager.GameState.GAMEOVER; }
        else { GameManager.Instance.state = GameManager.GameState.WAVEEND; }
    }

    IEnumerator SpawnEnemies(Spawn spawn)
    {
        int count = Mathf.RoundToInt(RPNEvaluator.Evaluate(spawn.count,
        new Dictionary<string, float>{
            { "wave", CurrentWaveNumber },
            { "base", 0 }
            }
        ));
        int hp = Mathf.RoundToInt(RPNEvaluator.Evaluate(spawn.hp, new Dictionary<string, float>{
            { "wave", CurrentWaveNumber },
            { "base", EnemyDatabase.Instance.GetEnemyData(spawn.enemy).hp }
            }
        ));
        int speed = Mathf.RoundToInt(RPNEvaluator.Evaluate(spawn.speed, new Dictionary<string, float>{
            { "wave", CurrentWaveNumber },
            { "base", EnemyDatabase.Instance.GetEnemyData(spawn.enemy).speed }
            }
        ));
        int damage = Mathf.RoundToInt(RPNEvaluator.Evaluate(spawn.damage, new Dictionary<string, float>{
            { "wave", CurrentWaveNumber },
            { "base", EnemyDatabase.Instance.GetEnemyData(spawn.enemy).damage }
            }
        ));
        int spriteNumber = EnemyDatabase.Instance.GetEnemyData(spawn.enemy).sprite;
        float delay = float.Parse(spawn.delay);

        SpawnPoint[] customSpawnPoints;
        string[] locationParts = spawn.location.Split(' ');
        string locationPart = locationParts[locationParts.Length - 1];

        if (locationPart != "random")
        {
            customSpawnPoints = SpawnPoints.Where(s => s.name.ToLower().StartsWith(locationPart.ToLower())).ToArray();
        }
        else
        {
            customSpawnPoints = SpawnPoints;
        }

        int spawned = 0;
        int sequenceIndex = 0;

        while (spawned < count)
        {
            int spawnThisBatch = Mathf.Min(spawn.sequence[(sequenceIndex % spawn.sequence.Count)], count - spawned);
            for (int i = 0; i < spawnThisBatch; i++)
            {
                SpawnEnemy(customSpawnPoints, spriteNumber, hp, speed, damage);
                spawned++;
            }

            sequenceIndex++;
            yield return new WaitForSeconds(delay);
        }
    }

    void SpawnEnemy(SpawnPoint[] customSpawnPoints, int spriteNumber, int hp, int speed, int damage)
    {
        SpawnPoint spawn_point = customSpawnPoints[Random.Range(0, customSpawnPoints.Length)];
        Vector2 offset = Random.insideUnitCircle * 1.8f;
        Vector3 initial_position = spawn_point.transform.position + new Vector3(offset.x, offset.y, 0);

        GameObject new_enemy = Instantiate(enemy, initial_position, Quaternion.identity);
        new_enemy.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.enemySpriteManager.Get(spriteNumber);

        EnemyController en = new_enemy.GetComponent<EnemyController>();
        en.hp = new Hittable(hp, Hittable.Team.MONSTERS, new_enemy);
        en.speed = speed;
        en.damage = damage;

        GameManager.Instance.AddEnemy(new_enemy);
    }
}
