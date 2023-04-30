using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.GUI
{
    /// <summary>
    /// Component that takes care of the die screen effect
    /// </summary>
    public class DieScreen : MonoBehaviour
    {
        /// <summary>
        /// Finds the cannonical die screen for the provided gameobject - currently always a singleton found by <see cref="Object.FindAnyObjectByType{T}"/>
        /// </summary>
        /// <param name="self">Game object that's searching for its die screen.</param>
        /// <returns>Die screen to be used by the gameobject.</returns>
        public static DieScreen Get(GameObject self) => GameObject.FindAnyObjectByType<DieScreen>();

        /// <summary>
        /// How many seconds for the effect to fade-in
        /// </summary>
        public float FadeInDuration = 1f;
        /// <summary>
        /// Starts the die screen effect
        /// </summary>
        public void StartAndFadeIn()
        {
            gameObject.SetActive(true);
            var group = GetComponent<CanvasGroup>();
            group.alpha = 0f;
            group.DOFade(1f, FadeInDuration);
        }
    }
}