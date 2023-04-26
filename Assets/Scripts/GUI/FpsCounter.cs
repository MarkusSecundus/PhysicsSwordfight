using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.GUI
{
    public class FpsCounter : MonoBehaviour
    {
        TMP_Text text;
        [SerializeField] string format = "FPS: {0}";
        void Start()
        {
            text = GetComponent<TMP_Text>();
        }

        void Update()
        {
            float fps = 1f / Time.unscaledDeltaTime;
            text.text = string.Format(format, Mathf.RoundToInt(fps));
        }
    }
}