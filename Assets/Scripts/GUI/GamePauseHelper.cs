using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.GUI
{
    public class GamePauseHelper : MonoBehaviour
    {
        public GameObject GameRoot, PauseRoot;

        private float originalTimeScale;

        private bool isPaused = false;

        public void DoPause()
        {
            originalTimeScale = Time.timeScale;
            Debug.Log($"Saving original time scale: {originalTimeScale}");
            Time.timeScale = 0f;
            isPaused = true;
            GameRoot?.SetActive(false);
            PauseRoot?.SetActive(true);
        }

        public void DoUnpause()
        {
            Time.timeScale = originalTimeScale;
            GameRoot?.SetActive(true);
            PauseRoot?.SetActive(false);
            isPaused = false;
        }

        public void SwitchPausedState()
        {
            if (isPaused) DoUnpause();
            else DoPause();
        }
    }
}