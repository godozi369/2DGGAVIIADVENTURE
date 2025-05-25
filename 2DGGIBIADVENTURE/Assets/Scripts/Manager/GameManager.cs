using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public string spawnPointName = "";
    public GameObject player;
    public PlayerController playerController;
    public UIManager uiManager;

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

    public void SetSpawnPoint(string pointName)
    {
        spawnPointName = pointName;
    }

    public void PositionPlayerAtSpawn()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (!string.IsNullOrEmpty(spawnPointName))
        {
            GameObject spawnPoint = GameObject.Find(spawnPointName);
            if (spawnPoint != null && player != null)
            {
                Debug.Log($"✅ 이동! {player.name} → {spawnPointName} 위치: {spawnPoint.transform.position}");
                player.transform.position = spawnPoint.transform.position;
            }
            else
            {
                Debug.LogError($"❌ 스폰 실패: {spawnPointName} 못 찾음");
            }
        }
    }

    public void RegisterPlayer(PlayerController pc)
    {
        playerController = pc;
        player = pc.gameObject;
    }

    public void RegisterUI(UIManager ui)
    {
        uiManager = ui;
    }
}
