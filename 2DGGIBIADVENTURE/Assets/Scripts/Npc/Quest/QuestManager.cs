using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    public QuestData currentQuest;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartQuest(QuestData quest)
    {
        currentQuest = quest;
        quest.isCompleted = false;
    }
    public void ReportProgress(string targetID)
    {
        GameObject ozi = GameObject.Find("Npc_Ozi");
        ozi.GetComponent<PathFollower>().StartPath();

        if (currentQuest == null || currentQuest.isCompleted) return;

        if (currentQuest.questType == QuestType.KillByName &&
            currentQuest.targetID == targetID)
        {
            currentQuest.requiredCount--;

            if (currentQuest.requiredCount <= 0)
            {
                CompleteQuest(currentQuest);
            }
        }
    }

    public void CompleteQuest(QuestData quest)
    {
        currentQuest.isCompleted = true;
    }
}
