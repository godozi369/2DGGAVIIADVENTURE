using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAi : MonoBehaviour
{
    public enum State
    {
        None,
        Roaming,
        Follow,
    }

    public State state = State.Roaming;

    private Vector2 currentDir = Vector2.zero;
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Speed Setting")]
    public float roamSpeed = 1f;
    public float followSpeed = 2f;

    public Transform followTarget;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (state == State.Roaming)
            StartCoroutine(RoamingRoutine());
    }

    private void Update()
    {

        switch (state)
        {
            case State.Roaming:
                rb.velocity = currentDir * roamSpeed;
                break;
            case State.Follow:
                
                break;
            default:
                rb.velocity = Vector2.zero;
                break;
        }
    }
    private void FixedUpdate()
    {
        if (state == State.Roaming)
        {
            rb.MovePosition(rb.position + currentDir * (roamSpeed * Time.fixedDeltaTime));
        }
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

    public void StartFollow(Transform target)
    {
        followTarget = target;
        state = State.Follow;
    }
}
