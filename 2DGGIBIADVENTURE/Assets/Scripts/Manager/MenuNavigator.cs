using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MenuNavigator : MonoBehaviour
{
    public Transform selector; 
    public Transform[] menuTargets; 
    public UnityEvent[] onSelectEvents;
    public GameObject keyInfoPanel;

    private int currentIndex = 0;

    private void Start()
    {
        UpdateSelector();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentIndex = (currentIndex + 1) % menuTargets.Length;
            UpdateSelector();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentIndex = (currentIndex - 1 + menuTargets.Length) % menuTargets.Length;
            UpdateSelector();
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            onSelectEvents[currentIndex]?.Invoke();
        }
        if (keyInfoPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseSettings();
        }
    }

    private void UpdateSelector()
    {
        selector.position = new Vector3(
            selector.position.x,
            menuTargets[currentIndex].position.y,
            selector.position.z
        );
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Scene1");
    }

    public void OpenSettings()
    {
        
        keyInfoPanel.SetActive(true);
    }
    public void CloseSettings()
    {
        keyInfoPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Debug.Log("게임 종료");
        Application.Quit();
    }
}
