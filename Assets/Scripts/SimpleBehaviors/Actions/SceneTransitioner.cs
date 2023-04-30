using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MarkusSecundus.PhysicsSwordfight.Actions
{
    /// <summary>
    /// Component that provides a <see cref="UnityEvent"/>able action for transitioning between sceens
    /// </summary>
    public class SceneTransitioner : MonoBehaviour
    {
        /// <summary>
        /// Transition to scene with the given name
        /// </summary>
        /// <param name="sceneName">Name of the scene to transition to</param>
        public void TransitionToScene(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }
}