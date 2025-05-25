using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ProjectileController : MonoBehaviour
{
    public float lifetime;
    public event Action<Hittable,Vector3> OnHit;
    public ProjectileMovement movement;

    public int pierceCount = 1;
    private int hitsRemaining;
    private HashSet<Collider2D> alreadyHit = new HashSet<Collider2D>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hitsRemaining = pierceCount;
    }

    // Update is called once per frame
    void Update()
    {
        movement.Movement(transform);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("projectile")) return;
        if (collision.gameObject.CompareTag("unit"))
        {
            var col = collision.collider;
            if (alreadyHit.Contains(col)) return;
            alreadyHit.Add(col);

            var ec = collision.gameObject.GetComponent<EnemyController>();
            if (ec != null)
            {
                OnHit(ec.hp, transform.position);
            }
            else
            {
                var pc = collision.gameObject.GetComponent<PlayerController>();
                if (pc != null)
                {
                    OnHit(pc.hp, transform.position);
                }
            }

        }
        hitsRemaining--;
        if (hitsRemaining <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetLifetime(float lifetime)
    {
        StartCoroutine(Expire(lifetime));
    }

    public void SetPierceCount(int count)
    {
        pierceCount = count;
    }
    IEnumerator Expire(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
