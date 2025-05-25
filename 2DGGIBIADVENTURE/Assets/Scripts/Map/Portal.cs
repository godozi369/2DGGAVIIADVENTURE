using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IPortal
{
    public string targetSceneName;
    public string spawnPointName;

    public void UsePortal()
    {
        GameManager.Instance.SetSpawnPoint(spawnPointName);
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(targetSceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameManager.Instance.PositionPlayerAtSpawn();
        SceneManager.sceneLoaded -= OnSceneLoaded; 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            collision.GetComponent<PlayerController>().SetNearbyPortal(this);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            collision.GetComponent<PlayerController>().ClearNearbyPortal();
    }
}
