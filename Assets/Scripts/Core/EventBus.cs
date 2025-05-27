using UnityEngine;
using System;

public class EventBus
{
    private static EventBus theInstance;
    public static EventBus Instance
    {
        get
        {
            if (theInstance == null)
                theInstance = new EventBus();
            return theInstance;
        }
    }

    public event Action<Vector3, Damage, Hittable> OnDamage;
    public event Action OnWaveEnd;
    public event Action OnAllEnemyDeath;

    public event Action<Damage> OnAllEnemyHit;


    public void WhenAllEnemyDie()
    {
        OnAllEnemyDeath?.Invoke();
    }
    public void WaveEnded()
    {
        Debug.Log("The wave has endedededededededed.");
        OnWaveEnd?.Invoke();
    }
    public void AllEnemyAreHit(Damage dmg)
    {
        OnAllEnemyHit?.Invoke(dmg);
    }

    public void DoDamage(Vector3 where, Damage dmg, Hittable target)
    {
        OnDamage?.Invoke(where, dmg, target);
    }

}
