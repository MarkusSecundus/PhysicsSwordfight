using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public float Value
    {
        get => slider.normalizedValue;
        set => slider.normalizedValue = Mathf.Clamp01(value);
    }
}
