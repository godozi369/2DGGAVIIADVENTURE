using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public float damage = 0.5f;
    public float knockbackForce = 3f;
    public bool destroyAfterHit = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerData player = other.GetComponent<PlayerData>();
            if (player != null)
            {
                Vector2 dir = (other.transform.position - transform.position).normalized;
                player.TakeDamage(damage, dir, knockbackForce);

                Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
                }

                if (destroyAfterHit)
                    Destroy(gameObject);
            }
        }
    }
}
