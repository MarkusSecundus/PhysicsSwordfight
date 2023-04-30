using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MarkusSecundus.PhysicsSwordfight.GUI
{
    /// <summary>
    /// Component providing utility functions for things like exiting the game or restarting the current scene
    /// </summary>
    public class GameManagementActionsHelper : MonoBehaviour
    {
        float initialTimeScale;
        private void Awake()
        {
            if (initialTimeScale <= 0f)
                initialTimeScale = Time.timeScale;
        }

        /// <summary>
        /// Exits the game
        /// </summary>
        public void ExitGame()
        {
            Application.Quit();
        }
        /// <summary>
        /// Restarts the current scene
        /// </summary>
        public void RestartScene()
        {
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
            Time.timeScale = 1f;
        }
    }
}