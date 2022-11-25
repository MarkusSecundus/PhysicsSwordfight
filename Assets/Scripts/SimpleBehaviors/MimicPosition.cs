using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicPosition : MonoBehaviour
{
    public Transform toMimic;
    public Vector3 offset = Vector3.zero;

    [SerializeField] private bool computeOffset = false;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (computeOffset) offset = transform.position - toMimic.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (rb == null) transform.position = toMimic.position + offset;
    }

    private void FixedUpdate()
    {
        if (rb != null) rb.position = toMimic.position + offset;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + offset);
    }
}
