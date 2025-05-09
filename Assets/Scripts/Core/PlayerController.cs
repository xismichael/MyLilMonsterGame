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
    public SpellUIContainer spellUIContainer;

    public int speed;
    public Unit unit;

    public string role;

    private RoleClass currentRole;

    void Start()
    {
        unit = GetComponent<Unit>();
        GameManager.Instance.player = gameObject;
        role = "mage";
    }

    public void StartLevel()
    {
        spellcaster = new SpellCaster(125, 8, Hittable.Team.PLAYER);
        StartCoroutine(spellcaster.ManaRegeneration());

        hp = new Hittable(100, Hittable.Team.PLAYER, gameObject);
        hp.OnDeath += Die;
        hp.team = Hittable.Team.PLAYER;

        healthui.SetHealth(hp);
        manaui.SetSpellCaster(spellcaster);
        spellUIContainer.LoadUI(spellcaster.spells);
        currentRole = RoleClassDatabase.Instance.GetRoleClass(role);
        setStats(currentRole);
        GameManager.Instance.OnWaveEnd += () => setStats(currentRole);
    }

    void Update()
    {
        // Movement handled by OnMove
    }

    void setStats(RoleClass role)
    {
        spellcaster.mana = Mathf.RoundToInt(RPNEvaluator.Evaluate(role.mana, GetRPNVariables()));
        spellcaster.mana_reg = Mathf.RoundToInt(RPNEvaluator.Evaluate(role.mana_regeneration, GetRPNVariables()));
        spellcaster.power = Mathf.RoundToInt(RPNEvaluator.Evaluate(role.spellpower, GetRPNVariables()));
        hp.SetMaxHP(Mathf.RoundToInt(RPNEvaluator.Evaluate(role.health, GetRPNVariables())));
        speed = Mathf.RoundToInt(RPNEvaluator.Evaluate(role.speed, GetRPNVariables()));
    }

    public Dictionary<string, float> GetRPNVariables()
    {
        return new Dictionary<string, float>
        {
            { "wave", EnemySpawner.CurrentWaveNumber }
        };
    }

    void OnAttack(InputValue value)
    {
        if (GameManager.Instance.state == GameManager.GameState.PREGAME ||
            GameManager.Instance.state == GameManager.GameState.WAVEEND ||
            GameManager.Instance.state == GameManager.GameState.GAMEOVER) return;

        Vector2 mouseScreen = Mouse.current.position.value;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0;

        StartCoroutine(spellcaster.Cast(transform.position, mouseWorld));
    }

    void OnMove(InputValue value)
    {
        if (GameManager.Instance.state == GameManager.GameState.PREGAME ||
            GameManager.Instance.state == GameManager.GameState.WAVEEND ||
            GameManager.Instance.state == GameManager.GameState.GAMEOVER) return;

        unit.movement = value.Get<Vector2>() * speed;
    }

    void OnChangeSpell(InputValue value)
    {
        spellUIContainer.spellUIs[spellcaster.spellCastIndex].GetComponent<SpellUI>().highlight.SetActive(false);
        spellcaster.spellCastIndex = (spellcaster.spellCastIndex + 1) % spellcaster.GetCurrentSpellAmount();
    }

    void Die()
    {
        GameManager.Instance.state = GameManager.GameState.GAMEOVER;
        GameManager.Instance.PlayerDeath = true;
        EnemySpawner.Instance.StopAllCoroutines();

        var allEnemies = FindObjectsOfType<EnemyController>();
        foreach (var ec in allEnemies)
        {
            GameManager.Instance.RemoveEnemy(ec.gameObject);
            Destroy(ec.gameObject);
        }

    }
}
