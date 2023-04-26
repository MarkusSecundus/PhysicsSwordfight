using DG.Tweening;
using MarkusSecundus.PhysicsSwordfight.Symbols;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerDeathEffect : MonoBehaviour
{
    public Transform CameraDestination;
    public float MovementDuration;
    public FloatSymbol PopupDuration = new FloatSymbol { Default = 1.3f };

    public UnityEvent OnPopup;

    public void DoPlay()
    {
        float popupDuration = PopupDuration.Get();
        float deathDuration = Mathf.Max(MovementDuration, popupDuration) * 2f;

        transform.SetParent(null);
        CameraDestination.SetParent(null);
        transform.DOScale(CameraDestination.localScale, MovementDuration);
        transform.DORotateQuaternion(CameraDestination.rotation, MovementDuration);
        transform.DOMove(CameraDestination.position, MovementDuration);
        this.PerformWithDelay(OnPopup.Invoke, popupDuration);
        this.PerformWithDelay(() => { Destroy(transform.gameObject); Destroy(CameraDestination.gameObject); }, deathDuration);
    }
}
