using MarkusSecundus.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerActivityInfo : MonoBehaviour
{
    public int ActiveTriggerCollidersCount => activeTriggerCollidersCount;
    public int ActiveNormalCollidersCount => activeNormalCollidersCount;
    public IReadOnlyCollection<Collider> GetActiveTriggers() => activeTriggers.Keys;




    private Dictionary<Collider, int> activeTriggers = new Dictionary<Collider, int>();

    private int activeTriggerCollidersCount = 0, activeNormalCollidersCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (activeTriggers.TryGetValue(other, out var count)) activeTriggers[other] = ++count;
        else
        {
            activeTriggers[other] = 1;
            ++(other.isTrigger ? ref activeTriggerCollidersCount : ref activeNormalCollidersCount);
            Debug.Log($"BEGIN[{this.name} - {other.gameObject.name}]");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!activeTriggers.TryGetValue(other, out var count) || count < 1)
            throw new System.InvalidOperationException($"This shouldn't happen - exiting collider {other.name} which wasn't present in the active trigger set of {this.name}");
        if (--count < 1)
        {
            activeTriggers.Remove(other);
            --(other.isTrigger ? ref activeTriggerCollidersCount : ref activeNormalCollidersCount);
            Debug.Log($"  end[{this.name} - {other.gameObject.name}]");
        }
        else
            activeTriggers[other] = count;
    }
}
