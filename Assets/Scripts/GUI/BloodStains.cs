using DG.Tweening;
using MarkusSecundus.PhysicsSwordfight.Utils.Geometry;
using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using MarkusSecundus.PhysicsSwordfight.Utils.Randomness;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MarkusSecundus.PhysicsSwordfight.GUI
{
    /// <summary>
    /// Component responsible for displaying bloodstains effect on the screen.
    /// </summary>
    public class BloodStains : MonoBehaviour
    {
        /// <summary>
        /// The bloodstains image to be shown.
        /// </summary>
        [SerializeField] Image InstancePrefab;
        /// <summary>
        /// Variants how the blodstains can be rotated.
        /// </summary>
        [SerializeField] Quaternion[] Rotations = new Quaternion[] { Quaternion.Euler(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 90f), Quaternion.Euler(0f, 0f, 180f), Quaternion.Euler(0f, 0f, 270f) };

        new RectTransform transform => (RectTransform)base.transform;

        private readonly System.Random rand = new System.Random();

        /// <summary>
        /// Displays the bloodstains effect on the screen.
        /// </summary>
        /// <param name="totalDuration">How long the effect should take in total. Must be bigger than <paramref name="buildupTime"/>.</param>
        /// <param name="buildupTime">How many seconds for the image to reach peak alpha.</param>
        /// <param name="maxAlpha">Alpha the effect image will have on its peak.</param>
        public void Show(float totalDuration, float? buildupTime = null, float maxAlpha = 1f)
        {
            var buildup = buildupTime ??= Mathf.Min(0.1f, totalDuration * 0.1f);

            var instance = Instantiate(InstancePrefab);
            instance.rectTransform.SetParent(transform);
            instance.gameObject.SetActive(true);
            if (Rotations.Length > 0) instance.rectTransform.rotation = Rotations[rand.Next(0, Rotations.Length)];

            var range = instance.rectTransform.GetRect().PositionsWherePlacingThisRectCoversTheWholeOfSmallerRect(transform.GetRect());
            instance.rectTransform.position = rand.NextVector2(range);

            var color = instance.color;
            instance.color = color.With(a: 0f);
            instance.DOColor(color.With(a: color.a * maxAlpha), buildup).OnComplete(() => instance.DOColor(color.With(a: 0f), totalDuration - buildup).OnComplete(() => Destroy(instance.gameObject)));
        }
    }

}