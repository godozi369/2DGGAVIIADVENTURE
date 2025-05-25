using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public float maxHp = 3f;
    public float moveSpeed = 2f;
    public float chaseRange = 5f;
    public float attackRange = 1f;
    public float attackDuration = 0.5f;
    public float attackCooldown = 1f;
    public float attackDamage = 1f;
    public float knockbackForce = 3f;
    public float knockbackTime = 0.3f;
}
