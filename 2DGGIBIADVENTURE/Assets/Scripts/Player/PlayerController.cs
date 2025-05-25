using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private static bool exists = false;

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float dashSpeed = 6f;
    [SerializeField] private TrailRenderer trailRenderer;

    [SerializeField] private AudioClip attackSFX;
    [SerializeField] private AudioClip dashSFX;
    private AudioSource audioSource;

    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isDashing = false;
    // 히트박스
    [SerializeField] private GameObject hitboxUp;
    [SerializeField] private GameObject hitboxDown;
    [SerializeField] private GameObject hitboxLeft;
    [SerializeField] private GameObject hitboxRight;
    // 스킬
    [SerializeField] private SkillSlot[] skillSlots;
    // 투척 
    [SerializeField] private GameObject throwPrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private float throwForce = 10f;
    private Vector2 inputDirection;
    // 공격 방향 고정용 변수
    private float attackDirX;
    private float attackDirY;

    public NPCInteraction npcInRange;
    private PlayerKnockback knockback;
    private IPortal nearbyPortal;
    private bool isPlayerNearbyPortal = false;

    private void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        knockback = GetComponent<PlayerKnockback>();

        if (exists)
        {
            Destroy(gameObject);
            return;
        }
        exists = true;
        DontDestroyOnLoad(gameObject);
        
        GameManager.Instance?.RegisterPlayer(this);
    }
    private void Start()
    {
        playerControls.Combat.Attack.performed += _ => TryAttack();
        playerControls.Combat.Dash.performed += _ => Dash();
        playerControls.Interaction.Interaction.performed += _ => TryInteract();
        playerControls.Combat.Throw.performed += _ => HandleThrow();

        playerControls.Skill.Q.performed += _ => UseSkillByKey(KeyCode.Q);
        playerControls.Skill.W.performed += _ => UseSkillByKey(KeyCode.W);
        playerControls.Skill.E.performed += _ => UseSkillByKey(KeyCode.E);
        playerControls.Skill.R.performed += _ => UseSkillByKey(KeyCode.R);
    }
    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }
    private void Update()
    {
        if (knockback != null && knockback.IsKnockingBack)
        {
            return;
        }
        PlayerInput();

        if (playerControls.Interaction.Interaction.WasPressedThisFrame())
        {
            if (UIManager.Instance.dialoguePanel.activeSelf)
            {
                UIManager.Instance.AdvanceDialogue(); 
            }
            else if (npcInRange != null)
            {
                npcInRange.TriggerDialogue(); 
            }
            else if (isPlayerNearbyPortal && nearbyPortal != null)
            {
                nearbyPortal.UsePortal();
            }
        }
    }
    private void FixedUpdate()
    {
        if (knockback != null && knockback.IsKnockingBack)
        {
            return; 
        }
        Move();
        if (!animator.GetBool("IsAttacking"))
        {
            AdjustPlayerFacingDirection();
        }
    }
    private void PlayerInput()
    {
        movement = playerControls.MoveMent.Move.ReadValue<Vector2>();
        animator.SetBool("IsMoving", movement != Vector2.zero);
        // 최신 입력 방향 저장 (대각선 포함)
        if (movement != Vector2.zero)
        {
            inputDirection = movement.normalized;
        }
    }
    #region 이동
    private void Move()
    {
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.deltaTime));
    }
    private void AdjustPlayerFacingDirection()
    {
        if (movement == Vector2.zero) return; 

        if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
        {
            animator.SetFloat("MoveX", movement.x > 0 ? 1 : -1);
            animator.SetFloat("MoveY", 0);
        }
        else
        {
            animator.SetFloat("MoveX", 0);
            animator.SetFloat("MoveY", movement.y > 0 ? 1 : -1);
        }
    }

    #endregion
    #region 공격
    private void TryAttack()
    {
        if (!animator.GetBool("IsAttacking"))
            NormalAttack();
    }
    private void NormalAttack()
    {
        AdjustPlayerFacingDirection();
        // 바라보는 방향 고정해서 공격
        attackDirX = animator.GetFloat("MoveX");
        attackDirY = animator.GetFloat("MoveY");

        // 공격 애니메이션 고정
        animator.SetFloat("MoveX", attackDirX); 
        animator.SetFloat("MoveY", attackDirY); 

        animator.SetBool("IsAttacking", true);
        audioSource.PlayOneShot(attackSFX);

        EnableHitBoxInDirection();

        StartCoroutine(EndAttackRoutine());
    }
    // 공격 쿨타임 
    private IEnumerator EndAttackRoutine()
    {
        yield return new WaitForSeconds(0.2f); // 히트박스 활성화시간

        // 히트박스 끄기
        hitboxUp.SetActive(false);
        hitboxDown.SetActive(false);
        hitboxLeft.SetActive(false);
        hitboxRight.SetActive(false);

        yield return new WaitForSeconds(0.4f); // 남은 공격 애니메이션 시간 

        animator.SetBool("IsAttacking", false);
    }
    // 히트박스 
    private void EnableHitBoxInDirection()
    {
        hitboxUp.SetActive(false);
        hitboxDown.SetActive(false);
        hitboxLeft.SetActive(false);
        hitboxRight.SetActive(false);

        if (Mathf.Abs(attackDirX) > Mathf.Abs(attackDirY))
        {
            if (attackDirX > 0)
                hitboxLeft.SetActive(true);
            else
                hitboxRight.SetActive(true);
        }
        else
        {
            if (attackDirY > 0)
                hitboxUp.SetActive(true);
            else
                hitboxDown.SetActive(true);
        }
    }

    #endregion
    #region 대쉬
    private void Dash()
    {
        if (!isDashing)
        {
            audioSource.PlayOneShot(dashSFX);
            isDashing = true;
            moveSpeed *= dashSpeed;
            trailRenderer.emitting = true;
            StartCoroutine(EndDashRoutine());
        }
    }
    private IEnumerator EndDashRoutine()
    {
        float dashTime = .2f;
        float dashCD = .25f;
        yield return new WaitForSeconds(dashTime);
        moveSpeed /= dashSpeed;
        trailRenderer.emitting = false;
        yield return new WaitForSeconds(dashCD);
        isDashing = false;
    }

    #endregion
    #region 상호작용
    private void TryInteract()
    {
        if (npcInRange == null) return;
        
        npcInRange.TriggerDialogue();
    }

    public void SetNearbyPortal(IPortal portal)
    {
        nearbyPortal = portal;
        isPlayerNearbyPortal = true;
    }

    public void ClearNearbyPortal()
    {
        nearbyPortal = null;
        isPlayerNearbyPortal = false;
    }
    #endregion
    #region 투척
    private void PlayerThrow(GameObject prefab, float force, float rotationOffset = -90f, float spawnOffset = 0f)
    {
        Vector2 dir = inputDirection;

        if (dir == Vector2.zero)
        {
            float x = animator.GetFloat("MoveX");
            float y = animator.GetFloat("MoveY");
            dir = new Vector2(x, y).normalized;
        }

        if (dir == Vector2.zero)
        {
            dir = Vector2.down;
        }

        Vector2 spawnPos = (Vector2)throwPoint.position + dir * spawnOffset;
        GameObject projectile = Instantiate(prefab, spawnPos, Quaternion.identity);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = dir * force;
        }
    }
    private void HandleThrow()
    {
        animator.SetTrigger("Throw");
        PlayerThrow(throwPrefab, throwForce);
    }
    #endregion
    #region 스킬

    private void UseSkillByKey(KeyCode key)
    {
        foreach (var slot in skillSlots)
        {
            if (slot == null || slot.skill == null) continue;

            if (slot.key == key && slot.icon != null && slot.icon.gameObject.activeInHierarchy)
            {
                UseSkill(slot.skill);
                break;
            }
        }
    }
    private void UseSkill(SkillData skill)
    {
        animator.SetTrigger("Throw");
        PlayerThrow(skill.prefab, skill.speed, skill.rotationOffset, skill.spawnOffset);
    }
    #endregion

}
