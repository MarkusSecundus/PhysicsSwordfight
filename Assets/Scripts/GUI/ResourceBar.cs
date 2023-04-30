using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MarkusSecundus.PhysicsSwordfight.GUI
{
    /// <summary>
    /// GUI component for managing a resource bar.
    /// 
    /// <para>
    /// Requires a <see cref="Slider"/> to be present in some of its children.
    /// </para>
    /// </summary>
    public class ResourceBar : MonoBehaviour
    {
        Slider slider;
        void Awake()
        {
            slider = GetComponentInChildren<Slider>();
        }

#if false
    private void Update()
    {
        Value = (-Time.time).Mod(1f);
    }
#endif
        /// <summary>
        /// Value of the slider - member of interval [0;1]
        /// </summary>
        public float Value
        {
            get => slider.normalizedValue;
            set => slider.normalizedValue = Mathf.Clamp01(value);
        }
    }
}