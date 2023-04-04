using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerDeathEffect : MonoBehaviour
{
    public Transform CameraDestination;
    public float MovementDuration, PopupDuration;
    public float DeathDuration => Mathf.Max(MovementDuration, PopupDuration) * 2f;

    public UnityEvent OnComplete;

    public void DoPlay()
    {
        transform.SetParent(null);
        CameraDestination.SetParent(null);
        transform.DOScale(CameraDestination.localScale, MovementDuration);
        transform.DORotateQuaternion(CameraDestination.rotation, MovementDuration);
        transform.DOMove(CameraDestination.position, MovementDuration);
        this.PerformWithDelay(OnComplete.Invoke, PopupDuration);
        this.PerformWithDelay(() => { Destroy(transform.gameObject); Destroy(CameraDestination.gameObject); }, DeathDuration);
    }
}
