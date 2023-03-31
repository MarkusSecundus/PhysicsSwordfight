using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CollisionEventSendingFix : MonoBehaviour
{
    private void Awake()
    {
        if (!GetComponent<Rigidbody>()) Debug.LogError($"{typeof(CollisionEventSendingFix)} must be attached to the root rigidbody", this);
    }

    private void OnCollisionEnter(Collision collision)
        => DoSend(nameof(OnCollisionEnter), collision);
    private void OnCollisionStay(Collision collision)
        => DoSend(nameof(OnCollisionStay), collision);
    private void OnCollisionExit(Collision collision)
        => DoSend(nameof(OnCollisionExit), collision);

    void DoSend(string methodName, Collision argument)
    {
        var thisCollider = argument.ThisCollider();
        thisCollider.SendMessage(methodName, argument, SendMessageOptions.DontRequireReceiver);
    }
}
