using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DrawBridgeCableEffect : MonoBehaviour
{
    public Transform SegmentsRoot;

    public UnityEvent OnFinished;

    public Vector3 scaleMultipliers = new Vector3(1.3f, 1.3f, 1.3f);

    public float segmentDuration = 0.5f, segmentRecoverDuration = 1f;

    public Ease MainEase = Ease.OutBounce, RecoveryEase = Ease.Linear;

    private bool inProgress = false;
    public void StartTheEffect()
    {
        if (inProgress) return;
        inProgress = true;

        StartCoroutine(impl());
        IEnumerator<YieldInstruction> impl()
        {
            foreach (Transform segment in SegmentsRoot)
            {
                var originalScale = segment.localScale;
                segment.DOScale(Vector3.Scale(originalScale, scaleMultipliers), segmentDuration)
                    .OnComplete(() => segment.DOScale(originalScale, segmentRecoverDuration).SetEase(RecoveryEase).Play())
                    .Play();
                yield return new WaitForSeconds(segmentDuration);
            }
            OnFinished.Invoke();
            inProgress = false;
        }
    }
}
