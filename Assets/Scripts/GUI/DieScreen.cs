using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieScreen : MonoBehaviour
{
    public float FadeInDuration = 1f;
    public void StartAndFadeIn()
    {
        gameObject.SetActive(true);
        var group = GetComponent<CanvasGroup>();
        group.alpha = 0f;
        group.DOFade(1f, FadeInDuration);
    }
}
