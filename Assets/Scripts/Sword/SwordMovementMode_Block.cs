using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SwordMovementMode_Block : IScriptSubmodule<SwordMovement>
{
    public Transform BlockingPosition;
    public float BlockBeginDuration = 0.5f;
    public float BlockEndDuration = 0.3f;


    public SwordMovementMode_Block(SwordMovement script) : base(script){}

    public override void OnStart()
    {
        joint = Script.joint;
        originalConnectedAnchor = joint.connectedAnchor;
        joint.autoConfigureConnectedAnchor = false;
    }
    public override void OnActivated()
    {
        //StartBlock();
    }

    public override void OnDeactivated()
    {
        //EndBlock();
    }


    public Vector3 swordHandlePoint => Script.FixedSwordHandlePoint;
    private float swordLength => Script.SwordLength;
    public override void OnFixedUpdate(float delta)
    {

        Cursor.lockState = CursorLockMode.Confined;
        var input = Script.GetUserInput(swordHandlePoint,swordLength);

        if (!input.HasNullElement())
        {
            var hitPoint = input.First.Value;
            Script.SetAnchorPosition(hitPoint, float.NaN);
            Script.SetDebugPointPosition(hitPoint);
        }
    }

    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(swordHandlePoint, 0.01f);

        DrawHelpers.DrawWireSphere(swordHandlePoint, swordLength, Gizmos.DrawLine);
    }


    private ConfigurableJoint joint;

    private Vector3 originalConnectedAnchor;
    private TweenerCore<Vector3, Vector3, VectorOptions> tween = null;

    void StartBlock()
    {
        if (tween != null) { tween.Kill(); tween = null; }
        var endValue = originalConnectedAnchor + BlockingPosition.localPosition;
        tween = joint.DOConnectedAnchor(endValue, BlockBeginDuration);
    }
    void EndBlock()
    {
        tween?.Kill();
        tween = null;
        tween = joint.DOConnectedAnchor(originalConnectedAnchor, BlockBeginDuration);
    }

}
