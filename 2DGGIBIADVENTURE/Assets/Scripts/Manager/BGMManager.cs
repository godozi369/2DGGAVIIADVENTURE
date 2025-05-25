using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BGMManager : MonoBehaviour
{
    public static BGMManager instance;

    private AudioSource audioSource;

    public AudioClip defaultBGM;
    public List<SceneBGM> sceneBGMs = new List<SceneBGM>();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        SceneManager.sceneLoaded += OnSceneLoaded;

        PlayBGM(defaultBGM);
    }
    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (var entry in sceneBGMs)
        {
            if (entry.sceneName == scene.name && entry.bgm != null)
            {
                PlayBGM(entry.bgm);
                return;
            }
        }
        PlayBGM(defaultBGM);
    }

    public void PlayBGM(AudioClip clip)
    {
        if (audioSource.clip == clip)
        {
            return;
        }
        audioSource.clip = clip;
        audioSource.Play();
    }
    public void StopBGM()
    {
        audioSource.Stop();
    }
    public void SetVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp01(volume); 
    }
}

[System.Serializable]
public class SceneBGM
{
    public string sceneName;
    public AudioClip bgm;
}