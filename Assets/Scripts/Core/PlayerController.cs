using UnityEngine;
using UnityEngine.InputSystem;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Hittable hp;
    public HealthBar healthui;
    public ManaBar manaui;

    public SpellCaster spellcaster;
    public SpellUIContainer spellUIContainer;
    public RelicManager relicManager;

    public int speed;
    public Unit unit;

    public string role;
    public SpriteRenderer characterSpriteRenderer;

    private RoleClass currentRole;

    public float lastMoveTime;

    public event Action<float> OnStandStill;
    public event Action<Vector2> OnMoveEvent;
    public event Action<Hittable> OnHealthChange;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootingAudio;
    [SerializeField] private AudioClip travellingAudio;
    [SerializeField] private AudioClip attackAudio;

    //inventory related
    public SpellCraftingManager spellCraftingManager;
    private bool inventoryActive = false;
    private bool inventoryOpenAllowed = false;

    //cast related
    private bool castAllowed = true;

    void Start()
    {
        unit = GetComponent<Unit>();
        audioSource = gameObject.AddComponent<AudioSource>();
        GameManager.Instance.player = gameObject;
        role = "mage";
    }

    public void PlayShootingNoise(){
        audioSource.clip = shootingAudio;
        audioSource.PlayOneShot(shootingAudio);
    }

    public void PlayTravellingNoise(){
        audioSource.clip = travellingAudio;
        audioSource.PlayOneShot(travellingAudio);
    }

    public void PlayAttackAudio(){
        audioSource.clip = attackAudio;
        audioSource.PlayOneShot(attackAudio);
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
        //for roles
        currentRole = RoleClassDatabase.Instance.GetRoleClass(role);
        setStats(currentRole);
        EventBus.Instance.OnWaveEnd += () => setStats(currentRole);
        characterSpriteRenderer.sprite = GameManager.Instance.playerSpriteManager.Get(RoleClassDatabase.Instance.GetSpriteIcon(role));


        //for relics
        relicManager.Register(this);
        relicManager.RemoveAllRelics(this);

        //testing
        //spellcaster.power = 100;
        //relicManager.AddRelic("Predator's Grace");
        //relicManager.AddRelic("Jade Elephant");
        //spellCraftingManager.AddSpellToInventory(spellcaster.CreateStartSpell());
        //spellCraftingManager.AddSpellToInventory(spellcaster.CreateSpell("arcane_spray"));
        spellCraftingManager.AddSpellToInventory(spellcaster.CreateSpell("arcane_blast"));


        //for inventory
        inventoryOpenAllowed = true;
    }

    void Update()
    {
        // Movement handled by OnMove
        OnStandStill?.Invoke(Time.time - lastMoveTime);
        if (hp != null) OnHealthChange?.Invoke(hp);
        //Debug.Log(speed);
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
            GameManager.Instance.state == GameManager.GameState.GAMEOVER ||
            !castAllowed) return;

        Vector2 mouseScreen = Mouse.current.position.value;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0;

        //testing
        //Debug.Log("hp before heal: " + hp.hp);
        //hp.Heal(10);
        //Debug.Log("hp after heal: " + hp.hp);

        PlayShootingNoise();
        StartCoroutine(spellcaster.Cast(transform.position, mouseWorld));
    }

    void OnMove(InputValue value)
    {
        if (GameManager.Instance.state == GameManager.GameState.PREGAME ||
            GameManager.Instance.state == GameManager.GameState.WAVEEND ||
            GameManager.Instance.state == GameManager.GameState.GAMEOVER) return;

        lastMoveTime = Time.time;
        PlayTravellingNoise();
        unit.movement = value.Get<Vector2>() * speed;
        OnMoveEvent?.Invoke(value.Get<Vector2>());
    }

    void OnChangeSpell(InputValue value)
    {

        spellUIContainer.spellUIs[spellcaster.spellCastIndex].GetComponent<SpellUI>().highlight.SetActive(false);
        spellcaster.spellCastIndex = (spellcaster.spellCastIndex + 1) % spellcaster.GetCurrentSpellAmount();
    }

    void OnInventory(InputValue value)
    {
        if (!inventoryOpenAllowed) return;
        if (!inventoryActive)
        {
            spellCraftingManager.initialize();
            inventoryActive = true;
            castAllowed = false;
        }
        else
        {
            spellCraftingManager.close();
            inventoryActive = false;
            castAllowed = true;
        }
    }

    void Die()
    {
        GameManager.Instance.state = GameManager.GameState.GAMEOVER;
        GameManager.Instance.PlayerDeath = true;
        EnemySpawner.Instance.StopAllCoroutines();

        var allEnemies = FindObjectsOfType<EnemyController>();
        foreach (var ec in allEnemies)
        {
            PlayAttackAudio();
            GameManager.Instance.RemoveEnemy(ec.gameObject);
            Destroy(ec.gameObject);
        }

        spellCraftingManager.gameObject.SetActive(false);
        inventoryActive = false;
        castAllowed = true;
    }
}
