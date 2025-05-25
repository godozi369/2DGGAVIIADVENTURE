using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestDialogueSet
{
    public QuestData quest;
    public string[] startLines;
    public string[] inProgressLines;
    public string[] completeLines;
}
