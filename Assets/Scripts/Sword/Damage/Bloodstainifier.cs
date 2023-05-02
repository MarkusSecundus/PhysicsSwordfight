using DG.Tweening;
using MarkusSecundus.Utils;
using MarkusSecundus.Utils.Datastructs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Sword.Damage
{
    /// <summary>
    /// Cosmetic effect for visualizing <see cref="Damageable"/> entity's health state. Intended to display bloodstains on their body, now just colors them red.
    /// </summary>
    public class Bloodstainifier : MonoBehaviour
    {
        /// <summary>
        /// All renderer components to be affected by the effect
        /// </summary>
        [SerializeField] Renderer[] Renderers;
        /// <summary>
        /// Color that is being added as HP decreases
        /// </summary>
        [SerializeField] Color ColorOfBlood = new Color(0.7f, 0f, 0f);
        /// <summary>
        /// How long the update effect takes each time the component gets damaged
        /// </summary>
        [SerializeField] float TransitionDuration = 0.3f;

        DefaultValDict<Material, Color> originalColors = new DefaultValDict<Material, Color>(m => m.color);

        /// <summary>
        /// Callback for updating the bloodstains. Register this in <see cref="Damageable.OnUpdated"/> event.
        /// </summary>
        /// <param name="args">Stats describing the damage change</param>
        public void UpdateBloodstains(Damageable.HpChangedArgs args)
            => UpdateBloodstains(1 - args.Target.HP / args.Target.MaxHP);


        void UpdateBloodstains(float ratio)
        {
            foreach (var renderer in Renderers)
            {
                var mat = renderer.material;
                mat.DOColor(Color.Lerp(originalColors[mat], ColorOfBlood, ratio), TransitionDuration);
            }
        }
    }
}