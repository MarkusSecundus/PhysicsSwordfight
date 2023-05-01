using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using MarkusSecundus.Utils;
using MarkusSecundus.Utils.Datastructs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace MarkusSecundus.PhysicsSwordfight.Cosmetics
{
    /// <summary>
    /// Component responsible for performing various visual effects that can be started via a <see cref="UnityEvent"/> callback
    /// </summary>
    public class VisualEffectsHelper : MonoBehaviour
    {
        /// <summary>
        /// Renderers to be affected by the effects
        /// </summary>
        [SerializeField] Renderer[] AffectedRenderers;
        /// <summary>
        /// Parameters for the blinking effect
        /// </summary>
        [SerializeField] BlinkingArgs Blinking;
        /// <summary>
        /// Parameters for the blinking effect
        /// </summary>
        [System.Serializable]
        public struct BlinkingArgs
        {
            /// <summary>
            /// How many seconds the effect takes from start to finish
            /// </summary>
            public float TotalDuration;
            /// <summary>
            /// How many seconds from peak to fade. Must be lesser than <see cref="TotalDuration"/>
            /// </summary>
            public float FadeTime;
            /// <summary>
            /// Color with which the objects blink
            /// </summary>
            public Color Color;
        }

        private class InternalBlinkingInfo
        {
            public TweenerCore<Color, Color, ColorOptions> CurrentlyPlaying;
            public Color OriginalColor;
        }
        private DefaultValDict<Renderer, InternalBlinkingInfo> rendererMetadata = new DefaultValDict<Renderer, InternalBlinkingInfo>(r => new InternalBlinkingInfo());

        /// <summary>
        /// Start the blinking event. Parameters are obtained from <see cref="Blinking"/>
        /// </summary>
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