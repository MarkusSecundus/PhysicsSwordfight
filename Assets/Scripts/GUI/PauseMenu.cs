using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.GUI
{
    /// <summary>
    /// Component responsible for pausing the game and displaying Pause Menu
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        /// <summary>
        /// Root of the pause menu Canvas
        /// </summary>
        [SerializeField] GameObject Root;
        /// <summary>
        /// If the game is currently paused
        /// </summary>
        public bool IsPaused { get => Root.activeSelf; }

        float lastTimeScale;
        private void Awake()
        {
            lastTimeScale = Time.timeScale;
            Root.SetActive(false);
        }

        /// <summary>
        /// Start the pause menu and suspend the game
        /// </summary>
        public void DoPause()
        {
            if (IsPaused) return;
            lastTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            Root.SetActive(true);
        }

        /// <summary>
        /// Exit the pause menu and resume the game
        /// </summary>
        public void DoUnpause()
        {
            if (!IsPaused) return;
            Time.timeScale = lastTimeScale;
            Root.SetActive(false);
        }

        /// <summary>
        /// Switch between on/off states of the pause menu
        /// </summary>
        public void TogglePause()
        {
            if (IsPaused) DoUnpause();
            else DoPause();
        }
    }
}