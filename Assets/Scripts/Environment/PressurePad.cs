using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePad : MonoBehaviour
{

    public UnityEvent OnPressed;

    private Color originalColor;

    private MeshRenderer rend;

    private void Start()
    {
        rend = GetComponent<MeshRenderer>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        OnTriggerEnter(null);
    }
    private void OnCollisionExit(Collision collision)
    {
        OnTriggerExit(null);
    }

    private void OnTriggerEnter(Collider other)
    {
        originalColor = rend.material.color;
        rend.material.color = Color.red;

        OnPressed.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        rend.material.color = originalColor;
    }


}
