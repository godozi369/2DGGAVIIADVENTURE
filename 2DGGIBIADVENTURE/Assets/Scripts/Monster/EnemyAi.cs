using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAi : MonoBehaviour
{
    public EnemyData data;
    [SerializeField] private string targetID;
    private enum State { Idle, Roaming, Chasing, Attacking, Dead }
    private State state;

    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;

    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip deathSound;

    private AudioSource audioSource;

    private float currentHp;
    private bool isAttacking;
    private Vector2 currentDir = Vector2.zero;
    private float lastAttackTime = -999f;
    public bool isKnockBack { get; private set; } = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        currentHp = data.maxHp;
        state = State.Idle;
    }

    private void Start()
    {
        StartCoroutine(BeginRoamingAfterDelay());
    }
    private void Update()
    {
        if (state == State.Dead || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // 공격 가능할 때
        if (!isAttacking && Time.time >= lastAttackTime + data.attackCooldown && distance <= data.attackRange + 0.2f)
        {
            StartCoroutine(AttackRoutine());
            return;
        }
        else if (!isAttacking && distance < data.attackRange)
        {
            currentDir = Vector2.zero;
            state = State.Idle;
            return;
        }
        // 추격 조건 
        else if (!isAttacking && distance <= data.chaseRange)
        {
            state = State.Chasing;
            currentDir = (player.position - transform.position).normalized;
        }
        // 플레이어와 멀어졌을 때
        else if (!isAttacking)
        {
            state = State.Roaming;
        }
        
        
    }
    private void FixedUpdate()
    {
        if (state == State.Dead || isKnockBack) return;

        rb.MovePosition(rb.position + currentDir * (data.moveSpeed * Time.fixedDeltaTime));

        animator.SetBool("IsMoving", currentDir != Vector2.zero);

        if (currentDir != Vector2.zero)
        {
            animator.SetFloat("MoveX", currentDir.x);
            animator.SetFloat("MoveY", currentDir.y);
        }
    }

    // 시작 시 대기상태
    private IEnumerator BeginRoamingAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        state = State.Roaming;
        StartCoroutine(RoamingRoutine());
    }
    private IEnumerator RoamingRoutine()
    {
        while (true)
        {
            if (state == State.Roaming)
            {
                bool doIdle = Random.value < 0.5f;
                if (doIdle)
                {
                    currentDir = Vector2.zero;
                }
                else
                {
                    Vector2 roamDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                    currentDir = roamDir;
                }
            }

            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }
    private IEnumerator AttackRoutine()
    {
        state = State.Attacking;
        isAttacking = true;
        currentDir = Vector2.zero;

        animator.SetBool("IsAttacking", true);
        if (attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
        yield return new WaitForSeconds(data.attackDuration); // 공격 애니메이션 길이
        animator.SetBool("IsAttacking", false);

        lastAttackTime = Time.time; // 마지막 공격 시간 저장

        isAttacking = false;
        state = State.Chasing;
    }
    public void TakeDamage(float damage, Vector2 attackerPos)
    {
        if (state == State.Dead) return;

        currentHp -= damage;
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (currentHp <= 0)
        {
            rb.velocity = Vector2.zero;
            StartCoroutine(Die());
        }
        else
        {
            StartCoroutine(Hurt(attackerPos));
        }
    }
    
    private IEnumerator Hurt(Vector2 attackPos)
    {
        animator.SetTrigger("Hurt");

        isKnockBack = true;

        Vector2 knockDir = (transform.position - (Vector3)attackPos).normalized;
        rb.velocity = Vector2.zero;
        rb.AddForce(knockDir * data.knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(data.knockbackTime);

        rb.velocity = Vector2.zero;
        isKnockBack = false;
    }
    private IEnumerator Die()
    {
        QuestManager.Instance?.ReportProgress(targetID);

        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        state = State.Dead;
        animator.SetTrigger("Die");

        currentDir = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;

        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }
    public void ResetEnemy()
    {
        currentHp = data.maxHp;
        state = State.Idle;
        currentDir = Vector2.zero;
        lastAttackTime = -999f;
        isKnockBack = false;
        isAttacking = false;
        GetComponent<Collider2D>().enabled = true;
        gameObject.SetActive(true);
        animator.Play("Idle", -1, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data.attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, data.chaseRange);
    }
}
