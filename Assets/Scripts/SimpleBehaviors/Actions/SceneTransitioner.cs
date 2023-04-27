using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Actions
{
    public class SceneTransitioner : MonoBehaviour
    {
        public void TransitionToScene(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }
}