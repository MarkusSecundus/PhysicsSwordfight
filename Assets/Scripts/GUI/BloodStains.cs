using DG.Tweening;
using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using MarkusSecundus.PhysicsSwordfight.Utils.Randomness;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MarkusSecundus.PhysicsSwordfight.GUI
{
    public class BloodStains : MonoBehaviour
    {
        [SerializeField] Image InstancePrefab;
        [SerializeField] Quaternion[] Rotations = new Quaternion[] { Quaternion.Euler(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 90f), Quaternion.Euler(0f, 0f, 180f), Quaternion.Euler(0f, 0f, 270f) };

        new RectTransform transform => (RectTransform)base.transform;

        private readonly System.Random rand = new System.Random();

        public RectTransform Show(float totalDuration, float? buildupTime = null, float maxAlpha = 1f)
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

            return instance.rectTransform;
        }
    }

}