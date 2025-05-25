using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public List<Transform> pathPoints;
    public float moveSpeed = 2f;

    private Animator animator;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void StartPath()
    {
        StartCoroutine(FollowPath());
    }

    private IEnumerator FollowPath()
    {
        foreach (Transform point in pathPoints)
        {
            while (Vector2.Distance(transform.position, point.position) > 0.05f)
            {
                Vector2 dir = (point.position - transform.position).normalized;
                transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);

                animator.SetBool("IsMoving", true);
                animator.SetFloat("MoveX", dir.x);
                animator.SetFloat("MoveY", dir.y);

                yield return null;
            }

            transform.position = point.position;
        }
        animator.SetBool("IsMoving", false);
    }
}
