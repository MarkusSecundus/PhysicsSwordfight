using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillFloor : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
        => KillObject(other.gameObject);

    private void OnCollisionEnter(Collision collision)
        => KillObject(collision.gameObject);

    private void KillObject(GameObject o)
    {
        var player = o.GetComponentInParent<Respawnable>();
        if (player == null) return;

        player.Die();
    }
}
