using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MarkusSecundus.PhysicsSwordfight.Cosmetics
{
    public class FadeHelper : MonoBehaviour
    {
        public SpriteRenderer fader;

        public float duration;

        public Color beginColor, endColor;

        public UnityEvent OnFadeCompleted;

        public bool activeWhenEnded;


        public void DoFade()
        {
            fader.gameObject.SetActive(true);
            fader.color = beginColor;
            fader.DOColor(endColor, duration)
                .OnComplete(() =>
                {
                    fader.gameObject.SetActive(activeWhenEnded);
                    OnFadeCompleted.Invoke();
                })
                .Play();
        }
    }
}