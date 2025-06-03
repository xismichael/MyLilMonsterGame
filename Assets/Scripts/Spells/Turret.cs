using UnityEngine;
using System.Collections;

public class TurretBehavior : MonoBehaviour
{
    private Spell spell;
    private Hittable.Team team;
    private float attackInterval;
    private float lifetime;

    public void Initialize(Spell spell, Hittable.Team team, float attackInterval, float lifetime)
    {
        this.spell = spell;
        this.team = team;
        this.attackInterval = attackInterval;
        this.lifetime = lifetime;

        StartCoroutine(TurretRoutine());
    }

    private IEnumerator TurretRoutine()
    {
        float timer = 0f;

        while (timer < lifetime)
        {
            GameObject target = FindTarget();
            if (target != null)
            {
                // Important: Call CastBase, NOT Cast
                CoroutineManager.Instance.Run(((TurretSpell)spell).CastBase(transform.position, target.transform.position, team));
            }

            yield return new WaitForSeconds(attackInterval);
            timer += attackInterval;
        }

        Destroy(gameObject);
    }

    private GameObject FindTarget()
    {
        GameObject closest = GameManager.Instance.GetClosestEnemy(transform.position);
        return closest;
    }
}