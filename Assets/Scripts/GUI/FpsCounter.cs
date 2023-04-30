using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.GUI
{
    /// <summary>
    /// Component for displaying current Frames-Per-Second of the game
    /// </summary>
    public class FpsCounter : MonoBehaviour
    {
        TMP_Text text;
        /// <summary>
        /// Format of fps report - use <c>{0}</c> to mark the fps value
        /// </summary>
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