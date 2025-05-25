using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitBox : MonoBehaviour
{
    public float damage = 1f;
    public GameObject attacker;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"[충돌 발생] 충돌 대상: {collision.name}, 태그: {collision.tag}");
        if (collision.gameObject == attacker) return; // 자기 자신 제외

        if (attacker.CompareTag("Enemy") && collision.CompareTag("Enemy")) return; // 적 끼리 제외

        if (attacker == null) return;

        if (attacker.CompareTag("Enemy") && collision.CompareTag("Player"))
        {
            PlayerData player = collision.GetComponent<PlayerData>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
        else if (attacker.CompareTag("Player") && collision.CompareTag("Enemy"))
        {
            EnemyAi enemy = collision.GetComponent<EnemyAi>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, transform.position);
            }
            BossAi boss = collision.GetComponentInParent<BossAi>();
            if (boss != null)
            {
                Debug.Log("✅ BossAi 감지됨 — 데미지 호출!");
                boss.TakeDamage(damage);
            }
            else
            {
                Debug.LogWarning("❌ BossAi 못 찾음 — 충돌한 오브젝트: " + collision.name);
            }

        }
    }
}
