using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualEffectsHelper : MonoBehaviour
{
    [SerializeField] Renderer[] AffectedRenderers;
    [SerializeField] BlinkingArgs Blinking;
    [System.Serializable] public struct BlinkingArgs
    {
        public float TotalDuration, FadeTime;
        public Color Color;
    }


    private bool CurrentlyInProgress = false;
    public void Blink()
    {
        if (Op.post_assign(ref CurrentlyInProgress, true)) return;

        TweenerCore<Color, Color, ColorOptions> last = null;
        foreach(var renderer in AffectedRenderers)
        {
            var mat = renderer.material;
            var originalColor = mat.color;
            var tween = mat.DOColor(Blinking.Color, Blinking.TotalDuration - Blinking.FadeTime);
            tween.OnComplete(() => 
            {
                mat.DOColor(originalColor, Blinking.FadeTime); 
                CurrentlyInProgress = false; 
            });
        }
    }
}
