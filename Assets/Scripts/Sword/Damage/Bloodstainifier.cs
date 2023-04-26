using DG.Tweening;
using MarkusSecundus.Utils;
using MarkusSecundus.Utils.Datastructs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Sword.Damage
{
    public class Bloodstainifier : MonoBehaviour
    {
        [SerializeField] Renderer[] Renderers;
        [SerializeField] Color ColorOfBlood = new Color(0.7f, 0f, 0f);
        [SerializeField] float TransitionDuration = 0.3f;

        DefaultValDict<Material, Color> originalColors = new DefaultValDict<Material, Color>(m => m.color);

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