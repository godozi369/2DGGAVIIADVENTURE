using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingDealer : MonoBehaviour
{
    public float damage = 0.5f;

    [Header("옵션")]
    public bool destroyOnHit = true;
    public float lifeAfterHit = 0.5f;

    private bool hasHit = false;

    private void Start()
    {
        if (!destroyOnHit)
        {
            Destroy(gameObject, lifeAfterHit);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return; // 중복 충돌 방지

        if (collision.CompareTag("Enemy"))
        {
            var enemy = collision.GetComponent<EnemyAi>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, transform.position);
            }

            var boss = collision.GetComponentInParent<BossAi>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
            }

            HandleHit();
        }
        else if (!collision.CompareTag("Player"))
        {
            HandleHit();
        }
    }
    private void HandleHit()
    {
        hasHit = true;
        
        if (destroyOnHit)
        {
            Destroy(gameObject); // 수리검 즉시 제거
        }

        else
        {
            // 기능 비활성화
            GetComponent<Collider2D>().enabled = false;
            if (TryGetComponent<Rigidbody2D>(out var rb))
                rb.velocity = Vector2.zero;

            // 일정 시간 후 파괴 
            Destroy(gameObject, lifeAfterHit);
        }
    }
}
