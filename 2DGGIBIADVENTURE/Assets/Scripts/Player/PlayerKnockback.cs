using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnockback : MonoBehaviour
{
    private Rigidbody2D rb;
    public float knockbackDuration = 0.2f;
    private bool isKnockingBack = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        if (isKnockingBack) return;

        isKnockingBack = true;
        rb.velocity = Vector2.zero; 
        rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);

        Invoke(nameof(EndKnockback), knockbackDuration);
    }

    private void EndKnockback()
    {
        isKnockingBack = false;
    }

    public bool IsKnockingBack => isKnockingBack;
}
