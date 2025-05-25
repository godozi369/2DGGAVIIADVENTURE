using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BossHpUI : MonoBehaviour
{
    public Image hpImage;
    private float maxHp;
    private float targetFill = 1f;
    private float currentFill = 1f;
    [SerializeField] private float lerpSpeed = 6f;

    public void SetMaxHp(float hp)
    {
        maxHp = hp;
        targetFill = 1f;
        currentFill = 1f;
        hpImage.fillAmount = 1f;
    }

    public void SetHp(float currentHp)
    {
        targetFill = currentHp / maxHp;
    }

    private void Update()
    {
        // 부드럽게 
        currentFill = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * lerpSpeed);
        hpImage.fillAmount = currentFill;

        // 미세 오차 제거
        if (Mathf.Abs(currentFill - targetFill) < 0.001f)
        {
            currentFill = targetFill;
        }
    }
}
