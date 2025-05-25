using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BossAi : MonoBehaviour
{
    public BossData data;
    private float currentHp;
    private float skillTimer;
    private float specialTimer;
    private float lightningTimer;
    private Vector2 roamCenter;
    private Vector2 roamTarget;
    private bool isMoving = false;
    private Transform player;
    [SerializeField] private BossHpUI hpUI;
    [SerializeField] private float moveRange = 3f;
    private AudioSource audioSource;
    [SerializeField] private AudioClip hitSound;

    private Animator animator;
    private void Awake()
    {
        currentHp = data.maxHp;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        hpUI = FindObjectOfType<BossHpUI>(true);
    }
    private void Start()
    {
        hpUI.gameObject.SetActive(true);
        hpUI.SetMaxHp(currentHp);
        
        roamCenter = transform.position; 
        StartCoroutine(RoamRoutine());
    }

    private void Update()
    {
        float hpRate = currentHp / data.maxHp;
        Debug.Log("보스 HP 비율: " + hpRate);
        Debug.Log("보스 이동속도: " + data.moveSpeed);

        if (player == null) return;

        if (data.hasPhases)
        {
            skillTimer += Time.deltaTime;
            specialTimer += Time.deltaTime;
            lightningTimer += Time.deltaTime;

            // phase3
            if (hpRate <= data.phase3Threshold)
            {
                if (skillTimer >= data.skillInterval)
                {
                    skillTimer = 0f;
                    FireAtPlayer();
                }

                if (specialTimer >= data.specialPatternInterval)
                {
                    specialTimer = 0f;
                    StartCoroutine(FireInAllDirection(true));
                }

                if (lightningTimer >= data.lightningInterval)
                {
                    lightningTimer = 0f;
                    StartCoroutine(DoubleLightningStrikePattern());
                }
            }
            // phase 2
            else if (hpRate <= data.phase2Threshold && hpRate > data.phase3Threshold)
            {
                if (skillTimer >= data.skillInterval)
                {
                    skillTimer = 0f;
                    FireAtPlayer();
                }

                if (specialTimer >= data.specialPatternInterval)
                {
                    specialTimer = 0f;
                    StartCoroutine(FireInAllDirection());
                }

                if ( lightningTimer >= data.lightningInterval)
                {
                    lightningTimer = 0f;
                    StartCoroutine(LightningStrikePattern(15f));
                }
            }
            // phase 1
            else
            {
                if (skillTimer >= data.skillInterval)
                {
                    skillTimer = 0f;
                    FireAtPlayer();
                }

                if (lightningTimer >= data.lightningInterval)
                {
                    lightningTimer = 0f;
                    StartCoroutine(LightningStrikePattern(0f));
                }
            }
        }
    }
    private IEnumerator RoamRoutine()
    {
        while (true)
        {
            if (!isMoving)
            {
                roamTarget = roamCenter + UnityEngine.Random.insideUnitCircle * moveRange;
                isMoving = true;
            }

            transform.position = Vector2.MoveTowards(transform.position, roamTarget, data.moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, roamTarget) < 0.1f)
            {
                isMoving = false;
                float waitTime = UnityEngine.Random.Range(1f, 2f);
                yield return new WaitForSeconds(waitTime);
            }

            yield return null;
        }
    }

    public void TakeDamage(float damage)
    {
        audioSource.PlayOneShot(hitSound, 0.5f);
        currentHp -= damage;
        hpUI?.SetHp(currentHp);

        if (currentHp <= 0f)
        {
            Die();
        }
    }
    private void Die()
    {
        Destroy(gameObject, 1.5f);
    }
    
    #region FireCore
    private void FireAtPlayer()
    {
        Vector3 dir = (player.transform.position - transform.position).normalized;
        FireCore(dir);
    }

    private IEnumerator FireInAllDirection(bool isPhase3 = false)
    {
        int count = data.specialShotCount;
        int rounds = isPhase3 ? 5 : 3; 
        float angleOffsetStep = 15f; 
        float delay = 1f; 
        for (int r = 0; r < rounds; r++)
        {
            float offset = r * angleOffsetStep;
            for (int i = 0; i < count; i++)
            {
                float angle = (360f / count) * i + offset;
                Vector3 dir = Quaternion.Euler(0, 0, angle) * Vector3.right;

                FireCore(dir);
            }
            yield return new WaitForSeconds(delay);
        }
    }
    private void FireCore(Vector3 direction)
    {
        GameObject prefab = data.skillPrefabs[0];
        GameObject core = Instantiate(prefab, transform.position, quaternion.identity);
        core.GetComponent<Rigidbody2D>().AddForce(direction.normalized*data.skillSpeed, ForceMode2D.Impulse);
    }

    #endregion
    #region Lightning

    private IEnumerator LightningStrikePattern(float angleOffset)
    {
        List<GameObject> warnings = new List<GameObject>();
        List<Vector3> strikePoints = new List<Vector3>();
        List<Vector3> directions = new List<Vector3>(); // 방향 저장용

        float angleStep = 360f / data.lineStrikeCount;

        for (int i = 0; i < data.lineStrikeCount; i++)
        {
            float angle = angleStep * i + angleOffset;
            Vector3 dir = Quaternion.Euler(0, 0, angle) * Vector3.right;
            Vector3 pos = transform.position + dir * data.strikeRadius;

            Quaternion rot = Quaternion.FromToRotation(Vector3.left, dir);

            GameObject warning = Instantiate(data.warningLinePrefab, pos, rot);
            warnings.Add(warning);
            strikePoints.Add(pos);
            directions.Add(dir);
        }

        yield return new WaitForSeconds(data.warningDelay);

        for (int i = 0; i < strikePoints.Count; i++)
        {
            Destroy(warnings[i]);

            Quaternion rot = Quaternion.FromToRotation(Vector3.left, directions[i]);
            GameObject lightning = Instantiate(data.lightningStrikePrefab, strikePoints[i], rot);
            Destroy(lightning, data.lightningTime);
        }
    }
    private IEnumerator DoubleLightningStrikePattern()
    {
        yield return StartCoroutine(LightningStrikePattern(0f));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(LightningStrikePattern(15f));
    }

    #endregion
}