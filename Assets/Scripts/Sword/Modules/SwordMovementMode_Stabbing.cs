using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class SwordMovementMode_Stabbing : IScriptSubmodule<SwordMovement>
{
    public Transform SwordDirectionHint;
    public IRayIntersectable InputIntersector;
    public float StabDistance = 0.8f;
    public KeyCode StabKey = KeyCode.Mouse0;

    public SwordMovementMode_Stabbing(SwordMovement script) : base(script){}


    private Transform handleBegin => Script.Sword.SwordAnchor;
    private Transform bladeTip => Script.Sword.SwordTip;
    private Transform bladeEdgeBlockPoint => Script.Sword.SwordBlockPoint;
    public override void OnFixedUpdate(float delta)
    {
        var input = GetUserInput();
        if (input != null) 
            SetStabPosition(input.Value);
    }

    private Vector3? GetUserInput()
    {
        var inputRay = Script.Input.GetInputRay();
        //var input = (Input.GetKey(StabKey)?ref InputIntersectorWhenStabbing: ref InputIntersector).GetIntersection(inputRay.Value);
        var input = InputIntersector.GetIntersection(inputRay);
        if (input.IsValid)
            return input.Value;

        return null;
    }

    private void SetStabPosition(Vector3 hitPoint)
    {
        var bestDirectionHint = getBestDirectionHint(hitPoint);
        var handlePosition = hitPoint;
        var lookDirection = (bestDirectionHint - handlePosition).normalized;
        if (Input.GetKey(StabKey)) handlePosition += lookDirection * StabDistance;
        var swordRotation = Quaternion.LookRotation(lookDirection, Vector3.up);


        Script.MoveAnchorPosition(handlePosition);
        Script.SetSwordRotation(swordRotation);
    }


    private Vector3 getBestDirectionHint(Vector3 hitPoint)
    {
        return SwordDirectionHint.OfType<Transform>().Minimal(hint => hint.position.Distance(hitPoint)).position;
    }


    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        //InputIntersector.Visualize(Camera.main, Gizmos.DrawLine);
        //Gizmos.DrawSphere(fixedHandlePoint, 0.01f);

        //DrawHelpers.DrawWireSphere(fixedHandlePoint, SwordLength, Gizmos.DrawLine);
    }

}
