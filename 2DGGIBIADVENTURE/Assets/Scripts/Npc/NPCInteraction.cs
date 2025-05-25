using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum BalloonState
{
    None,
    Question,
    Talking,
    Exclamation
}

public class NPCInteraction : MonoBehaviour
{
    [Header("UI")]
    public GameObject balloon;
    public GameObject talkingBalloonObject;
    public Sprite questionSprite;
    public Sprite exclamationSprite;
    private SpriteRenderer balloonRenderer;

    [Header("퀘스트 및 대사")]
    [SerializeField] private QuestData quest;
    public List<QuestDialogueSet> dialogueSets;

    public string[] dialogueLines =
    {

    };
    private void Awake()
    {
        if (balloon != null)
        {
            balloonRenderer = balloon.GetComponent<SpriteRenderer>();
        }
    }
    private void Start()
    {
        balloonRenderer = balloon.GetComponent<SpriteRenderer>();
        UpdateBalloonState();
    }
    private void OnEnable()
    {
        UpdateBalloonState();
    }
    public void SetBalloonState(BalloonState state)
    {
        if (balloon == null || balloonRenderer == null) return;

        balloon.SetActive(true);
        balloonRenderer.enabled = false;
        if (talkingBalloonObject != null)
        {
            talkingBalloonObject.SetActive(false);
        }

        switch (state)
        {
            case BalloonState.None:
                balloon.SetActive(false); 
                break;
            case BalloonState.Question:
                balloonRenderer.enabled = true;
                balloonRenderer.sprite = questionSprite;
                break;
            case BalloonState.Exclamation:
                balloonRenderer.enabled = true;
                balloonRenderer.sprite = exclamationSprite;
                break;
            case BalloonState.Talking:
                if (talkingBalloonObject != null)
                    talkingBalloonObject.SetActive(true); 
                break;
        }
    }
    private void UpdateBalloonState()
    {
        if (UIManager.Instance != null && UIManager.Instance.IsDialoguePlaying)
        {
            SetBalloonState(BalloonState.Talking); 
            return;
        }
        if (quest == null)
        {
            SetBalloonState(BalloonState.None);
            return;
        }
        if (QuestManager.Instance == null || QuestManager.Instance.currentQuest == null)
        {
            SetBalloonState(BalloonState.Question);
        }
        else if (quest.isCompleted)
        {
            SetBalloonState(BalloonState.Exclamation);
        }
        else
        {
            SetBalloonState(BalloonState.Question); 
        }
    }
    public void TriggerDialogue()
    {
        if (UIManager.Instance.IsDialoguePlaying) return;

        QuestDialogueSet set = null;
        foreach (QuestDialogueSet dialogueSet in dialogueSets)
        {
            if (dialogueSet.quest == quest)
            {
                set = dialogueSet;
                break;
            }
        }

        if (set == null)
        {
            SetBalloonState(BalloonState.None);
            UIManager.Instance.StartDialogue(new string[] { "..." });
            return;
        }

        if (QuestManager.Instance.currentQuest != quest)
        {
            // 퀘스트 수락 전 
            SetBalloonState(BalloonState.Talking);
            UIManager.Instance.StartDialogue(set.startLines);
            QuestManager.Instance.StartQuest(quest);
        }
        else if (!quest.isCompleted)
        {
            // 진행 중인데 값이 null 이면 바로 complete
            if (set.inProgressLines == null || set.inProgressLines.Length == 0)
            {
                SetBalloonState(BalloonState.Exclamation);
                UIManager.Instance.StartDialogue(set.completeLines);
                QuestManager.Instance.CompleteQuest(quest);
            }
            else
            {
                // 진행 중 
                SetBalloonState(BalloonState.Talking);
                UIManager.Instance.StartDialogue(set.inProgressLines);
            }
        }
        else
        {
            // 완료
            SetBalloonState(BalloonState.Exclamation);
            UIManager.Instance.StartDialogue(set.completeLines);

            // 보상 또는 다음 퀘스트 준비
            QuestManager.Instance.CompleteQuest(quest);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().npcInRange = this;
            UpdateBalloonState();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().npcInRange = null;
        }
    }
}
