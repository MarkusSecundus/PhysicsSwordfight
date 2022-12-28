using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToJointConnectedAnchor : MimicPositionBase
{
    public ConfigurableJoint toMimic;
    protected override Vector3 PositionToMimic => toMimic.connectedBody.transform.LocalToGlobal(toMimic.connectedAnchor);

    public Vector3 jointPos;

    protected override void Update()
    {
        base.Update();

        jointPos = toMimic.connectedAnchor;
    }
}

