using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MarkusSecundus.PhysicsSwordfight.GUI
{
    public class GameManagementActionsHelper : MonoBehaviour
    {
        float initialTimeScale;
        private void Awake()
        {
            if (initialTimeScale <= 0f)
                initialTimeScale = Time.timeScale;
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void RestartScene()
        {
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
            Time.timeScale = 1f;
        }
    }
}