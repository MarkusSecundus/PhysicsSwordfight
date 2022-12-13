using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SwordMovement;

[System.Serializable]
public class SwordMovementMode_Block : IScriptSubmodule<SwordMovement>
{
    public Transform BlockingPosition;
    public float BlockBeginDuration = 0.5f;
    public float BlockEndDuration = 0.3f;


    public SwordMovementMode_Block(SwordMovement script) : base(script){}

    public override void OnStart()
    {
        joint = Script.GetComponent<ConfigurableJoint>();
        originalConnectedAnchor = joint.connectedAnchor;
    }
    public override void OnActivated()
    {
        joint.autoConfigureConnectedAnchor = false;
    }

    private ConfigurableJoint joint;

    private Vector3 originalConnectedAnchor;
    private TweenerCore<Vector3, Vector3, VectorOptions> tween = null;
    void ManageBlocking(float delta)
    {
        const KeyCode BlockKey = KeyCode.LeftShift;
        if (Input.GetKeyDown(BlockKey)) StartBlock();
        else if (Input.GetKeyUp(BlockKey)) EndBlock();

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


}
