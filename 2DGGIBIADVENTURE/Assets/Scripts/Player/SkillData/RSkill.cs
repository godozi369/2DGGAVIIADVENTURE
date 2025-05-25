using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSkill : MonoBehaviour
{
    public GameObject flameEffectPrefab;
    public GameObject explosionPrefab;
    public float damage = 2f;
    [Header("이펙트 세부 설정")]
    public int explosionPerDir = 6;
    public float explosionInterval = 0.2f;
    public float explosionSpacing = 1.2f;
    public float flameDistance = 1f;
    

    private void Start()
    {
        StartCoroutine(FireAndExplode());
    }

    private IEnumerator FireAndExplode()
    {
        // 1. 화염 애니메이션 전개
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Vector2 dir = rotation * Vector2.right;

            Vector2 basePos = transform.position;
            Vector2 flamePos = basePos + dir.normalized * flameDistance;
            Instantiate(flameEffectPrefab, flamePos, Quaternion.Euler(0, 0, angle));
        }

        yield return new WaitForSeconds(0.5f); // 이펙트 후 딜레이

        // 2. 각 방향 연쇄 폭발
        for (int i = 0; i < 12; i++)
        {
            StartCoroutine(ExplodeInDirection(i * 30f));
        }

        float maxDelay = explosionPerDir * explosionInterval + 0.5f;
        yield return new WaitForSeconds(maxDelay);

        Destroy(gameObject);
    }

    private IEnumerator ExplodeInDirection(float angle)
    {
        Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right;

        for (int i = 1; i <= explosionPerDir; i++)
        {
            if (Random.value < 0.3f)
            {
                yield return new WaitForSeconds(explosionInterval);
                continue;
            }

            float spacingOffset = explosionSpacing + Random.Range(-0.3f, 0.3f);
            float timeOffset = Random.Range(-0.3f, 0.3f);

            Vector2 pos = (Vector2)transform.position + dir * (i * explosionSpacing);
            Instantiate(explosionPrefab, pos, Quaternion.identity);

            // 데미지 감지
            Collider2D[] hits = Physics2D.OverlapCircleAll(pos, 1f);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    hit.GetComponent<EnemyAi>()?.TakeDamage(damage, pos);
                }
            }

            yield return new WaitForSeconds(explosionInterval + timeOffset);
        }
    }
}
