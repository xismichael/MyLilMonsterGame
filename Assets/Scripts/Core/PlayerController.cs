using UnityEngine;
using UnityEngine.InputSystem;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public Hittable hp;
    public HealthBar healthui;
    public ManaBar manaui;

    public SpellCaster spellcaster;
    public SpellUI spellui;

    public int speed;
    public Unit unit;

    public GameObject deathMessageUI; //assign the death message in inspector

    void Start()
    {
        unit = GetComponent<Unit>();
        GameManager.Instance.player = gameObject;
    }

    public void StartLevel()
    {
        spellcaster = new SpellCaster(125, 8, Hittable.Team.PLAYER);
        StartCoroutine(spellcaster.ManaRegeneration());

        // Hide death message when restarting
        if (deathMessageUI != null)
            deathMessageUI.SetActive(false);

        hp = new Hittable(100, Hittable.Team.PLAYER, gameObject);
        hp.OnDeath += Die;
        hp.team = Hittable.Team.PLAYER;

        healthui.SetHealth(hp);
        manaui.SetSpellCaster(spellcaster);
        spellui.SetSpell(spellcaster.spell);
    }

    void Update()
    {
        // Movement handled by OnMove
    }

    void OnAttack(InputValue value)
    {
        if (GameManager.Instance.state == GameManager.GameState.PREGAME ||
            GameManager.Instance.state == GameManager.GameState.GAMEOVER) return;

        Vector2 mouseScreen = Mouse.current.position.value;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0;

        StartCoroutine(spellcaster.Cast(transform.position, mouseWorld));
    }

    void OnMove(InputValue value)
    {
        if (GameManager.Instance.state == GameManager.GameState.PREGAME ||
            GameManager.Instance.state == GameManager.GameState.GAMEOVER) return;

        unit.movement = value.Get<Vector2>() * speed;
    }

    void Die()
    {
        GameManager.Instance.state = GameManager.GameState.GAMEOVER;

        var spawner = FindObjectOfType<EnemySpawner>();

        if (spawner.waveStatsText != null)
            spawner.waveStatsText.text = "";

        spawner.StopAllCoroutines();

        var allEnemies = FindObjectsOfType<EnemyController>();
        foreach (var ec in allEnemies)
        {
            GameManager.Instance.RemoveEnemy(ec.gameObject);
            Destroy(ec.gameObject);
        }

        //show YOU DIED message
        if (deathMessageUI != null)
            deathMessageUI.SetActive(true);

        spawner.level_selector.gameObject.SetActive(true);
        spawner.restartScreen();
    }
}
