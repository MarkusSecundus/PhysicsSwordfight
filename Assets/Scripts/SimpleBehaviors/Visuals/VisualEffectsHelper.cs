using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using MarkusSecundus.Utils;
using MarkusSecundus.Utils.Datastructs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MarkusSecundus.PhysicsSwordfight.Cosmetics
{
    public class VisualEffectsHelper : MonoBehaviour
    {
        [SerializeField] Renderer[] AffectedRenderers;
        [SerializeField] BlinkingArgs Blinking;
        [System.Serializable]
        public struct BlinkingArgs
        {
            public float TotalDuration, FadeTime;
            public Color Color;
        }

        private class InternalBlinkingInfo
        {
            public TweenerCore<Color, Color, ColorOptions> CurrentlyPlaying;
            public Color OriginalColor;
        }
        private DefaultValDict<Renderer, InternalBlinkingInfo> rendererMetadata = new DefaultValDict<Renderer, InternalBlinkingInfo>(r => new InternalBlinkingInfo());

        public void Blink()
        {
            foreach (var renderer in AffectedRenderers)
            {
                var data = rendererMetadata[renderer];
                var mat = renderer.material;
                if (data.CurrentlyPlaying == null)
                {
                    data.OriginalColor = mat.color;
                }
                data.CurrentlyPlaying = mat.DOColor(Blinking.Color, Blinking.TotalDuration - Blinking.FadeTime)
                    .OnComplete(() =>
                {
                    data.CurrentlyPlaying = mat.DOColor(data.OriginalColor, Blinking.FadeTime)
                            .OnComplete(() => data.CurrentlyPlaying = null);
                });
            }
        }
    }
}