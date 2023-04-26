using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.GUI
{
    public class DieScreen : MonoBehaviour
    {
        public static DieScreen Get() => GameObject.FindAnyObjectByType<DieScreen>();

        public float FadeInDuration = 1f;
        public void StartAndFadeIn()
        {
            gameObject.SetActive(true);
            var group = GetComponent<CanvasGroup>();
            group.alpha = 0f;
            group.DOFade(1f, FadeInDuration);
        }
    }
}