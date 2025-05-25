using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBook : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"충돌 감지: {other.gameObject.name}");
        if (other.CompareTag("Player"))
        {
            Debug.Log("스킬북 먹음");
            SkillManager.Instance?.UnlockNextSkill();
            Destroy(gameObject);
            return;
        }
    }
}
