using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Data/Quest")]
public class QuestData : ScriptableObject
{
    public QuestType questType;
    public string questID;
    public string questTitle;
    [TextArea] public string description;

    public int requiredCount; 
    public string targetID;     

    public bool isCompleted = false;

}