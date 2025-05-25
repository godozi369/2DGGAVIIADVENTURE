using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField] public float maxHp = 3;
    public float currentHp;

    private Animator animator;
    private AudioSource audioSource;
    [SerializeField] private AudioClip hitSound;

    private void Awake()
    {
        currentHp = maxHp;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        UIManager.Instance.RefreshHeartsUI((int)maxHp);
        UIManager.Instance.UpdateHearts(currentHp);  
    }

    public void TakeDamage(float damage, Vector2 knockbackDir = default, float knockbackForce = 0f)
    {
        audioSource.PlayOneShot(hitSound);
        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
        UIManager.Instance.UpdateHearts(currentHp);
        animator.SetTrigger("Hurt");

        if (knockbackForce > 0f)
        {
            GetComponent<PlayerKnockback>()?.ApplyKnockback(knockbackDir, knockbackForce);
        }
        if (currentHp <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        animator.SetTrigger("Die");

        // 재시작 처리 추가 
    }

    public void Heal(float amount)
    {
        if (currentHp >= maxHp) return;
        currentHp += amount;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
        UIManager.Instance.UpdateHearts(currentHp);
    }

    public void IncreaseMaxHp(float amount)
    {
        maxHp += amount;
        currentHp = maxHp;

        UIManager.Instance.RefreshHeartsUI((int)maxHp);
        UIManager.Instance.UpdateHearts(currentHp);
    }
}
