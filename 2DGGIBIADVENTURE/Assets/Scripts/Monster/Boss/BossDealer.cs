using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDealer : MonoBehaviour
{
    public float damage = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var player = collision.GetComponent<PlayerData>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (!collision.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
