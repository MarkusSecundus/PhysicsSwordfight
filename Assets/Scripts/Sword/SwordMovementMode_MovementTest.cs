using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SwordMovementMode_MovementTest : IScriptSubmodule<SwordMovement>
{
    public Transform BlockingPosition;
    public float BlockBeginDuration = 0.5f;
    public float BlockEndDuration = 0.3f;

    public bool ShouldReturnBack = true;

    public SwordMovementMode_MovementTest(SwordMovement script) : base(script) { }


    public Vector3 swordHandlePoint => Script.FixedSwordHandlePoint;
    private float swordLength => Script.SwordLength;
    public override void OnFixedUpdate(float delta)
    {

        Cursor.lockState = CursorLockMode.Confined;
        var input = Script.GetUserInput(swordHandlePoint, swordLength);

        if (!input.HasNullElement())
        {
            var hitPoint = input.First.Value;
            Script.SetAnchorPosition(hitPoint, float.NaN);
            Script.SetDebugPointPosition(hitPoint);
        }
    }

    public override void OnDeactivated()
    {
        if(ShouldReturnBack)
            Script.SetAnchorPosition(swordHandlePoint, float.NaN);
    }

    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(swordHandlePoint, 0.01f);

        DrawHelpers.DrawWireSphere(swordHandlePoint, swordLength, Gizmos.DrawLine);
    }
}
