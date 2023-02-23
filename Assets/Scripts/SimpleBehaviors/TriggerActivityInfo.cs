using MarkusSecundus.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerActivityInfo : MonoBehaviour
{
    public bool IsTriggered() => activeTriggers.Count > 0;
    public IReadOnlyCollection<Collider> GetActiveTriggers() => activeTriggers.Keys;

    private Dictionary<Collider, int> activeTriggers = new Dictionary<Collider, int>();

    private void OnTriggerEnter(Collider other)
    {
        if (activeTriggers.TryGetValue(other, out var count)) activeTriggers[other] = ++count;
        else
        {
            activeTriggers[other] = 1;
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
            Debug.Log($"  end[{this.name} - {other.gameObject.name}]");
        }
        else
            activeTriggers[other] = count;
    }
}
