using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartPickup : MonoBehaviour
{
    public float healAmount = 1f; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerData player = collision.GetComponent<PlayerData>();
            if (player.currentHp >= player.maxHp) return;          
                player.Heal(healAmount);
                Destroy(gameObject); 
            
        }
    }
}
