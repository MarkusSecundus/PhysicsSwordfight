using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MimicPositionBase : MonoBehaviour
{
    protected abstract Vector3 PositionToMimic { get; }

    public Vector3 offset = Vector3.zero;

    [SerializeField] private bool computeOffset = false;
    public Space Space = Space.World;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (computeOffset) offset = transform.position - PositionToMimic;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (rb == null || Space == Space.Self)
        {
            if(Space == Space.World)transform.position = PositionToMimic + offset;
            else if(Space == Space.Self)transform.localPosition = PositionToMimic + offset;
        }
    }

    protected virtual void FixedUpdate()
    {
        if (rb != null && Space == Space.World)
        {
            rb.position = PositionToMimic + offset;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + offset);
    }
}


public class MimicPosition : MimicPositionBase
{
    public Transform toMimic;
    protected override Vector3 PositionToMimic => toMimic.position;
}

