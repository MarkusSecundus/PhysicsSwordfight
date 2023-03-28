using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Checkpoint : MonoBehaviour
{
    public Transform SpawnPoint;

    public UnityEvent OnPlayerRespawn;

    private void OnTriggerEnter(Collider other)
    {
        var player = other.gameObject.GetComponentInParent<Respawnable>();
        if (player == null) return;

        player.LastCheckpoint = this;
    }
}
