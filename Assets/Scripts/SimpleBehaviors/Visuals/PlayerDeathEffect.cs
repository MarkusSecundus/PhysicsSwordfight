using DG.Tweening;
using MarkusSecundus.PhysicsSwordfight.Symbols;
using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MarkusSecundus.PhysicsSwordfight.Cosmetics
{
    /// <summary>
    /// Component taking care of death effect - camera flows above the player's head and then death screen pops up.
    /// </summary>
    public class PlayerDeathEffect : MonoBehaviour
    {
        /// <summary>
        /// Destination for the camera to reach
        /// </summary>
        public Transform CameraDestination;
        /// <summary>
        /// How many seconds the whole visual effect takes.
        /// </summary>
        public float MovementDuration;
        /// <summary>
        /// How many seconds until <see cref="OnPopup"/> event is fired. Must be lesser than <see cref="MovementDuration"/>
        /// </summary>
        public FloatSymbol PopupDuration = new FloatSymbol { Default = 1.3f };

        /// <summary>
        /// Event to fire after <see cref="PopupDuration"/> is reached.
        /// </summary>
        public UnityEvent OnPopup;

        /// <summary>
        /// Play the death effect
        /// </summary>
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
}