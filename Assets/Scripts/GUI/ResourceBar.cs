using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour
{
    Scrollbar scrollbar;
    // Start is called before the first frame update
    void Start()
    {
        scrollbar = GetComponent<Scrollbar>();
    }

    public float Value
    {
        get => scrollbar.size;
        set => scrollbar.size = Mathf.Clamp01(value);
    }
}
