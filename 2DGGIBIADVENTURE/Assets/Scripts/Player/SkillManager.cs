using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SkillSlot
{
    public KeyCode key;
    public SkillData skill;
    public Image icon;
    public GameObject rootObject;
}

[CreateAssetMenu(menuName = "Skill/SkillData")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public Sprite icon;
    public GameObject prefab;
    public float cooldown;
    public float damage;
    public float speed;
    public float rotationOffset = -90f;
    public float spawnOffset = 0.5f;
}

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;

    [SerializeField] private SkillSlot[] skillSlots; 
    [SerializeField] private SkillData[] availableSkills; 
    private int unlockedIndex = 0;

    private void Awake()
    {
        Instance = this;
        foreach (var slot in skillSlots)
            slot.rootObject.SetActive(false); // 각 슬롯 비활성화
    }

    public void UnlockNextSkill()
    {
        if (unlockedIndex >= skillSlots.Length) return;

        var slot = skillSlots[unlockedIndex];

        if (slot.rootObject != null)
        {
            slot.rootObject.SetActive(true); // UI 슬롯 활성화
        }
        else
        {
            Debug.LogWarning($"[SkillManager] rootObject가 null임 (index: {unlockedIndex})");
        }

        slot.skill = availableSkills[unlockedIndex]; // 스킬 데이터 할당
        unlockedIndex++;
    }
}
