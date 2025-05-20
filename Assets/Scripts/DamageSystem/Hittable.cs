using UnityEngine;
using System;

public class Hittable
{
    public enum Team { PLAYER, MONSTERS }
    public Team team;

    public int hp;
    public int max_hp;

    public event Action OnDeath;

    public event Action<Damage> OnHit;




    public GameObject owner;

    public void Damage(Damage damage)
    {
        EventBus.Instance.DoDamage(owner.transform.position, damage, this);

        //tracks player damage taken this wave
        if (team == Team.PLAYER)
        {
            EnemySpawner.Instance.currentWaveDamageTaken += damage.amount;
            EnemySpawner.Instance.TotalDamageTaken += damage.amount;
        }

        if (team == Team.MONSTERS)
        {
            EventBus.Instance.AllEnemyAreHit(damage);
        }

        hp -= damage.amount;
        OnHit?.Invoke(damage);
        if (hp <= 0)
        {
            hp = 0;
            if (team == Team.MONSTERS)
            {
                EventBus.Instance.WhenAllEnemyDie();
            }
            OnDeath();
        }
    }

    public Hittable(int hp, Team team, GameObject owner)
    {
        this.hp = hp;
        this.max_hp = hp;
        this.team = team;
        this.owner = owner;
    }

    public void SetMaxHP(int max_hp)
    {
        float perc = this.hp * 1.0f / this.max_hp;
        this.max_hp = max_hp;
        this.hp = Mathf.RoundToInt(perc * max_hp);
    }

    public void Heal(int amount)
    {
        hp = Mathf.Min(max_hp, hp + amount);
    }
}

