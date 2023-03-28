using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
    TMP_Text text;
    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        float fps = 1f / Time.unscaledDeltaTime;
        text.text = $"FPS: {Mathf.RoundToInt(fps)}";
    }
}
