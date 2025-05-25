using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WSkill : MonoBehaviour
{
    public float damage = 3f;
    public float explodeDelay = 1.5f;          
    public float delayBeforeDestroy = 0.1f;    
    public string explosionTrigger = "Explode";

    private Animator animator;
    private Collider2D col;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    private void Start()
    {
        StartCoroutine(Explode());
    }

    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(explodeDelay);

        if (transform.parent != null)
        {
            var parentRenderer = transform.parent.GetComponent<Renderer>();
            if (parentRenderer != null)
            {
                parentRenderer.enabled = false;
            }
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, col.bounds.extents.x);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                hit.GetComponent<EnemyAi>()?.TakeDamage(damage, transform.position);
            }
        }

        // 3. 애니메이션 재생
        animator?.SetTrigger(explosionTrigger);

        // 4. 오브젝트 삭제 (부모 포함)
        Destroy(transform.parent != null ? transform.parent.gameObject : gameObject, delayBeforeDestroy);
    }
}
