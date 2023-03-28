using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RuntimeAnchorOverride : MonoBehaviour
{
    public bool SetMin = false;
    public Vector2 Min;
    public bool SetMax = false;
    public Vector2 Max;
    public bool SetPivot = false;
    public Vector2 Pivot;

    void Start()
    {
        var t = (RectTransform)transform;
        Debug.Log($"Values were: min{t.anchorMin}, max: {t.anchorMax}, pivot{t.pivot}", this);
        if (SetMin) t.anchorMin = Min;
        if (SetMax) t.anchorMax = Max;
        if (SetPivot) t.pivot = Pivot;
        Debug.Log($"Now are: min{t.anchorMin}, max: {t.anchorMax}, pivot{t.pivot}", this);
    }
}
