using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseHeart : MonoBehaviour
{
    public float bonusAmount = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerData health = collision.GetComponent<PlayerData>();
            if (health != null)
            {
                health.IncreaseMaxHp(bonusAmount);
            }

            Destroy(gameObject); 
        }
    }
}
