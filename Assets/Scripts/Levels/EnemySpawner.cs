using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    public Image level_selector;
    public GameObject button;
    public GameObject enemy;
    public SpawnPoint[] SpawnPoints;

    private static int CurrentWaveNumber = 1;

    private string currentLevelName;

    void Start()
    {
        float yStart = 130;
        float ySpacing = -50;
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
        GameManager.Instance.player.GetComponent<PlayerController>().StartLevel();
        currentLevelName = levelname;
        StartCoroutine(SpawnWave(levelname));
    }

    public void NextWave()
    {
        // You’ll probably want to track and increment wave number here.
        CurrentWaveNumber++;
        StartCoroutine(SpawnWave(currentLevelName));
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

        foreach (Spawn spawn in level.spawns)
        {


            StartCoroutine(SpawnEnemies(spawn));
        }

        yield return new WaitWhile(() => GameManager.Instance.enemy_count > 0);
        GameManager.Instance.state = GameManager.GameState.WAVEEND;
    }

    IEnumerator SpawnEnemies(Spawn spawn)
    {

        Debug.Log(spawn.enemy);
        Debug.Log(EnemyDatabase.Instance.GetEnemyData(spawn.enemy).hp);
        int count = RPNEvaluator.Evaluate(spawn.count, 0, CurrentWaveNumber);
        int hp = RPNEvaluator.Evaluate(spawn.hp, EnemyDatabase.Instance.GetEnemyData(spawn.enemy).hp, CurrentWaveNumber);
        int speed = RPNEvaluator.Evaluate(spawn.speed, EnemyDatabase.Instance.GetEnemyData(spawn.enemy).speed, CurrentWaveNumber);
        int damage = RPNEvaluator.Evaluate(spawn.damage, EnemyDatabase.Instance.GetEnemyData(spawn.enemy).damage, CurrentWaveNumber);
        int spriteNumber = EnemyDatabase.Instance.GetEnemyData(spawn.enemy).sprite;
        float delay = float.Parse(spawn.delay);

        int spawned = 0;
        int sequenceIndex = 0;

        while (spawned < count)
        {
            int spawnThisBatch = Mathf.Min(spawn.sequence[sequenceIndex % spawn.sequence.Count], count - spawned);

            for (int i = 0; i < spawnThisBatch; i++)
            {
                SpawnEnemy(spriteNumber, hp, speed, damage);
                spawned++;
            }

            sequenceIndex++;
            yield return new WaitForSeconds(delay);
        }
    }

    void SpawnEnemy(int spriteNumber, int hp, int speed, int damage)
    {
        SpawnPoint spawn_point = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
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
