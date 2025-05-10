using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform target;
    public int speed;
    public int damage = 5;
    public Hittable hp;
    public HealthBar healthui;
    public bool dead;

    public float last_attack;

    // Start is called once before the first execution of update after the monoBehaviour is created
    void Start()
    {
        target = GameManager.Instance.player.transform;
        hp.OnDeath += Die;
        healthui.SetHealth(hp);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = target.position - transform.position;
        if (direction.magnitude < 2f)
        {
            DoAttack();
        }
        else
        {
            GetComponent<Unit>().movement = direction.normalized * speed;
        }
    }

    void DoAttack()
    {
        if (last_attack + 2 < Time.time)
        {
            last_attack = Time.time;
            target.gameObject.GetComponent<PlayerController>().hp.Damage(new Damage(damage, Damage.Type.PHYSICAL));
        }
    }

    void Die()
    {
        if (!dead)
        {
            dead = true;

            //counts enemy kill this wave
            EnemySpawner.Instance.currentWaveEnemiesKilled++;
            EnemySpawner.Instance.TotalEnemiesKilled++;

            GameManager.Instance.RemoveEnemy(gameObject);
            Destroy(gameObject);
        }
    }
}
