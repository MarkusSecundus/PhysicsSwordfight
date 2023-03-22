using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using callback = System.Action<UnityEngine.Collider>;

public class CallbackedTrigger : MonoBehaviour
{
    private callback OnEnter, OnExit;

    private List<Collider> colliders = new List<Collider>();
    public IEnumerable<Collider> Colliders => colliders;
    public CallbackedTrigger Add<T>(System.Action<T> initializer) where T: Collider
    {
        var collider = gameObject.AddComponent<T>();
        collider.isTrigger = true;
        initializer?.Invoke(collider);
        colliders.Add(collider);

        return this;
    }

    public CallbackedTrigger Init(int layer, callback onEnter=null, callback onExit = null)
    {
        gameObject.layer = layer;
        (OnEnter, OnExit) = (onEnter, onExit);

        return this;
    }

    private void OnTriggerEnter(Collider other) => OnEnter?.Invoke(other);
    private void OnTriggerExit(Collider other) => OnExit?.Invoke(other);
}