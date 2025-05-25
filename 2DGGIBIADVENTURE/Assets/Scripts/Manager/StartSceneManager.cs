using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneManager : MonoBehaviour
{
    public PathFollower[] pathFollowers;
    public float delay = 1f;
    private IEnumerator Start()
    {
        foreach (var follower in pathFollowers)
        {
            follower.StartPath();
            yield return new WaitForSeconds(delay);
        }
    }
}
