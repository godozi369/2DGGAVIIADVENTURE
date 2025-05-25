using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraConnector : MonoBehaviour
{
    private void Start()
    {
        var vcam = GetComponent<Cinemachine.CinemachineVirtualCamera>();
        var player = GameObject.FindGameObjectWithTag("Player");

        if (vcam != null && player != null)
        {
            vcam.Follow = player.transform;
        }
        else
        {
            Debug.LogWarning("VirtualCameraConnector: 플레이어나 VCam이 없습니다.");
        }
    }
}
