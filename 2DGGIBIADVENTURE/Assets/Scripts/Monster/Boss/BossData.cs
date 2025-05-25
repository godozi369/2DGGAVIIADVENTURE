using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Boss Data")]
public class BossData : ScriptableObject
{
    public string bossName = "Agis";
    public string targetID = "Boss";

    [Header("Stats")]
    public float maxHp = 100f;
    public float moveSpeed = 2f;
    public float attackDamage = 10f;
    public float attackCooldown = 2f;
    public float detectRange = 10f;

    [Header("Phase")]
    public bool hasPhases = false;
    public float phase2Threshold = 0.66f; 
    public float phase3Threshold = 0.33f; 

    [Header("Skills")]
    public GameObject[] skillPrefabs; 
    public float skillInterval = 2f;
    public float skillSpeed = 7f;

    [Header("Special Pattern")]
    public float specialPatternInterval = 8f;
    public int specialShotCount = 8;

    [Header("Lightning Strike Pattern")]
    public GameObject warningLinePrefab;
    public GameObject lightningStrikePrefab;
    public float lightningInterval = 6f;
    public float warningDelay = 0.5f;
    public float lightningTime = 3.0f;
    public int lineStrikeCount = 8;
    public float strikeRadius = 3f;

}
