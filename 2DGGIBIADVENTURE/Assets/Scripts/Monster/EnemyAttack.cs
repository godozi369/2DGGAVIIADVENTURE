using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public Transform player;

    public GameObject hitboxUp;
    public GameObject hitboxDown;
    public GameObject hitboxLeft;
    public GameObject hitboxRight;

    private Animator animator;
    private Vector2 lastDirection;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // 공격 애니메이션 중간에 이벤트로 호출
    public void EnableAttackHitbox()
    {
        DisableAllHitboxes();

        float dirX = animator.GetFloat("MoveX");
        float dirY = animator.GetFloat("MoveY");

        GameObject selectedHitbox = null;

        if (Mathf.Abs(dirX) > Mathf.Abs(dirY))
        {
            selectedHitbox = dirX > 0 ? hitboxRight : hitboxLeft;
        }
        else
        {
            selectedHitbox = dirY > 0 ? hitboxUp : hitboxDown;
        }

        if (selectedHitbox != null)
        {
            EnemyAi ai = GetComponent<EnemyAi>();
            AttackHitBox ahb = selectedHitbox.GetComponent<AttackHitBox>();
            ahb.attacker = gameObject;
            ahb.damage = ai.data.attackDamage;

            if (ai != null)
            {
                ahb.damage = ai.data.attackDamage;
            }

            selectedHitbox.SetActive(true);
        }

        StartCoroutine(DisableHitboxAfterDelay());
    }

    private IEnumerator DisableHitboxAfterDelay()
    {
        yield return new WaitForSeconds(0.2f); // 맞는 타이밍으로 조정
        DisableAllHitboxes();
    }

    private void DisableAllHitboxes()
    {
        hitboxUp.SetActive(false);
        hitboxDown.SetActive(false);
        hitboxLeft.SetActive(false);
        hitboxRight.SetActive(false);
    }
}
