using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrashJar : MonoBehaviour
{
    public GameObject[] dropItems; // 드랍리스트
    public float dropChance = 0.5f; // 확률

    private Animator animator;
    private bool isBroken = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Crash()
    {
        if (isBroken) return;
        
        isBroken = true;
        animator.SetTrigger("Crash");

        if (Random.value < dropChance)
        {
            int index = Random.Range(0, dropItems.Length);
            Instantiate(dropItems[index], transform.position, Quaternion.identity);
        }

        float animLength = animator.GetCurrentAnimatorStateInfo(0).length;
        StartCoroutine(DestroyAfterDelay(animLength));
    }
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("AttackHitBox"))
        {
            Crash();
        }
    }
}
