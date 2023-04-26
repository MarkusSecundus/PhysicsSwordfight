using MarkusSecundus.Utils;
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
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!activeTriggers.TryGetValue(other, out var count) || count < 1)
            Debug.LogError($"This shouldn't happen - exiting collider {other.name} which wasn't present in the active trigger set of {this.name}");
        if (--count < 1)
        {
            activeTriggers.Remove(other);
            --(other.isTrigger ? ref activeTriggerCollidersCount : ref activeNormalCollidersCount);
        }
        else
            activeTriggers[other] = count;
    }
}
