using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    private PlayerController player;

    public Transform heartParent;
    public GameObject heartPrefab;
    public Image[] hearts;

    public Sprite fullHeart;
    public Sprite HalfHeart;
    public Sprite emptyHeart;

    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI hintText;
    [SerializeField] private TextMeshProUGUI questText;

    private string[] currentLines;
    private int currentIndex = 0;
    public bool IsDialoguePlaying => dialoguePanel.activeSelf;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        GameManager.Instance?.RegisterUI(this);
    }
    private void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerController>();
        }
    }
    private void Update()
    {
        var quest = QuestManager.Instance.currentQuest;

        if (questText != null && quest != null)
        {
            if (!quest.isCompleted)
                questText.text = $"{quest.questTitle} \n ({quest.requiredCount} 남음)";
            else
                questText.text = "";
        }
    }

    public void RegisterPlayer(PlayerController playerController)
    {
        player = playerController;
    }

    #region UI Text
    public void StartDialogue(string[] lines)
    {
        currentLines = lines;
        currentIndex = -1;

        dialoguePanel.SetActive(true);
        hintText.gameObject.SetActive(true);
    }
    public void AdvanceDialogue()
    {
        currentIndex++;
        if (currentIndex < currentLines.Length)
        {
            dialogueText.text = currentLines[currentIndex];
        }
        else
        {
            dialoguePanel.SetActive(false);
            hintText.gameObject.SetActive(false);

            var player = GameManager.Instance?.playerController;
            if (player != null && player.npcInRange != null)
            {
                player.npcInRange.SetBalloonState(BalloonState.None);
            }
        }
    }

    #endregion
    #region UI HP
    public void UpdateHearts(float health)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health - 0.5f)
            {
                hearts[i].sprite = fullHeart;
            }
            else if (i < health)
                hearts[i].sprite = HalfHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }
    public void RefreshHeartsUI(int newMaxHp)
    {
        foreach (Transform child in heartParent)
        {
            Destroy(child.gameObject);
        }

        hearts = new Image[newMaxHp];

        for (int i = 0; i < newMaxHp; i++)
        {
            GameObject newHeart = Instantiate(heartPrefab, heartParent);

            hearts[i] = newHeart.GetComponent<Image>();
        }
    }

    #endregion
    
}
