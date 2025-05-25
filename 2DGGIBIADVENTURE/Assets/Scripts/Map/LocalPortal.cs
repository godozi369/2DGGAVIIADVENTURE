using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPortal : MonoBehaviour, IPortal
{
    public Transform targetPosition;

    public void UsePortal()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = targetPosition.position;
        }
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
