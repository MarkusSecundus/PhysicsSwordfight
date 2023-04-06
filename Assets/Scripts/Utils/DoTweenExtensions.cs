using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.CustomPlugins;
using DG.Tweening.Plugins.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DoTweenExtensions 
{
    public static TweenerCore<Vector3, Vector3, VectorOptions> DOConnectedAnchor(this ConfigurableJoint joint, Vector3 endValue, float duration) 
        => DOTween.To(() => joint.connectedAnchor, v => joint.connectedAnchor = v, endValue, duration).SetTarget(joint).SetUpdate(UpdateType.Fixed);

    public static TweenerCore<Vector3, Vector3, VectorOptions> DOTargetPosition(this ConfigurableJoint joint, Vector3 endValue, float duration) 
        => DOTween.To(() => joint.targetPosition, v => joint.targetPosition = v, endValue, duration).SetTarget(joint).SetUpdate(UpdateType.Fixed);
    public static TweenerCore<Vector3, Vector3, VectorOptions> DOTargetVelocity(this ConfigurableJoint joint, Vector3 endValue, float duration) 
        => DOTween.To(() => joint.targetVelocity, v => joint.targetVelocity = v, endValue, duration).SetTarget(joint).SetUpdate(UpdateType.Fixed);
    public static TweenerCore<Vector3, Vector3, VectorOptions> DOTargetAngularVelocity(this ConfigurableJoint joint, Vector3 endValue, float duration) 
        => DOTween.To(() => joint.targetAngularVelocity, v => joint.targetAngularVelocity = v, endValue, duration).SetTarget(joint).SetUpdate(UpdateType.Fixed);
    public static TweenerCore<Quaternion, Vector3, QuaternionOptions> DOTargetRotation(this ConfigurableJoint joint, Vector3 endValue, float duration) 
        => DOTween.To(() => joint.targetRotation, v => joint.targetRotation = v, endValue, duration).SetTarget(joint).SetUpdate(UpdateType.Fixed);
    public static TweenerCore<Quaternion, Quaternion, NoOptions> DOTargetRotation(this JointRotationHelper joint, Quaternion endValue, float duration) 
        => DOTween.To(PureQuaternionPlugin.Plug(), () => joint.CurrentRotation, v => joint.SetTargetRotation(v), endValue, duration).SetTarget(joint).SetUpdate(UpdateType.Fixed);




    public static TweenerCore<Quaternion, Vector3, QuaternionOptions> DOTargetRotation(this JointRotationHelper joint, Vector3 endValue, float duration) 
        => DOTween.To(() => joint.CurrentRotation, joint.SetTargetRotation, endValue, duration).SetTarget(joint).SetUpdate(UpdateType.Fixed);
}

public static class DoTweenDriveExtensions
{
    public static TweenerCore<float, float, FloatOptions> DOSlerpDrive_MaximumForce(this ConfigurableJoint joint, float endValue, float duration)
        => DOTween.To(() => joint.slerpDrive.maximumForce, v =>
        {
            var tmp = joint.slerpDrive;
            tmp.maximumForce = v;
            joint.slerpDrive = tmp;
        }, endValue, duration).SetTarget(joint).SetUpdate(UpdateType.Fixed);
    public static TweenerCore<float, float, FloatOptions> DOSlerpDrive_PositionDamper(this ConfigurableJoint joint, float endValue, float duration)
        => DOTween.To(() => joint.slerpDrive.maximumForce, v =>
        {
            var tmp = joint.slerpDrive;
            tmp.positionDamper = v;
            joint.slerpDrive = tmp;
        }, endValue, duration).SetTarget(joint).SetUpdate(UpdateType.Fixed);
    public static TweenerCore<float, float, FloatOptions> DOSlerpDrive_PositionSpring(this ConfigurableJoint joint, float endValue, float duration)
        => DOTween.To(() => joint.slerpDrive.maximumForce, v =>
        {
            var tmp = joint.slerpDrive;
            tmp.positionSpring = v;
            joint.slerpDrive = tmp;
        }, endValue, duration).SetTarget(joint).SetUpdate(UpdateType.Fixed);
    


    public static TweenerCore<float, float, FloatOptions> DOXDrive_MaximumForce(this ConfigurableJoint joint, float endValue, float duration)
        => DOTween.To(() => joint.xDrive.maximumForce, v =>
        {
            var tmp = joint.xDrive;
            tmp.maximumForce = v;
            joint.xDrive = tmp;
        }, endValue, duration).SetTarget(joint).SetUpdate(UpdateType.Fixed);
    public static TweenerCore<float, float, FloatOptions> DOXDrive_PositionDamper(this ConfigurableJoint joint, float endValue, float duration)
        => DOTween.To(() => joint.xDrive.positionDamper, v =>
        {
            var tmp = joint.xDrive;
            tmp.positionDamper = v;
            joint.xDrive = tmp;
        }, endValue, duration).SetTarget(joint).SetUpdate(UpdateType.Fixed);
    public static TweenerCore<float, float, FloatOptions> DOXDrive_PositionSpring(this ConfigurableJoint joint, float endValue, float duration)
        => DOTween.To(() => joint.xDrive.positionSpring, v =>
        {
            var tmp = joint.xDrive;
            tmp.positionSpring = v;
            joint.xDrive = tmp;
        }, endValue, duration).SetTarget(joint).SetUpdate(UpdateType.Fixed);
    public static TweenerCore<float, float, FloatOptions> DOYDrive_MaximumForce(this ConfigurableJoint joint, float endValue, float duration)
        => DOTween.To(() => joint.yDrive.maximumForce, v =>
        {
            var tmp = joint.yDrive;
            tmp.maximumForce = v;
            joint.yDrive = tmp;
        }, endValue, duration).SetTarget(joint).SetUpdate(UpdateType.Fixed);
    public static TweenerCore<float, float, FloatOptions> DOYDrive_PositionDamper(this ConfigurableJoint joint, float endValue, float duration)
        => DOTween.To(() => joint.yDrive.positionDamper, v =>
        {
            var tmp = joint.yDrive;
            tmp.positionDamper = v;
            joint.yDrive = tmp;
        }, endValue, duration).SetTarget(joint).SetUpdate(UpdateType.Fixed);
    public static TweenerCore<float, float, FloatOptions> DOYDrive_PositionSpring(this ConfigurableJoint joint, float endValue, float duration)
        => DOTween.To(() => joint.yDrive.positionSpring, v =>
        {
            var tmp = joint.yDrive;
            tmp.positionSpring = v;
            joint.yDrive = tmp;
        }, endValue, duration).SetTarget(joint).SetUpdate(UpdateType.Fixed);
    
    public static TweenerCore<float, float, FloatOptions> DOZDrive_MaximumForce(this ConfigurableJoint joint, float endValue, float duration)
        => DOTween.To(() => joint.zDrive.maximumForce, v =>
        {
            var tmp = joint.zDrive;
            tmp.maximumForce = v;
            joint.zDrive = tmp;
        }, endValue, duration).SetTarget(joint).SetUpdate(UpdateType.Fixed);
    public static TweenerCore<float, float, FloatOptions> DOZDrive_PositionDamper(this ConfigurableJoint joint, float endValue, float duration)
        => DOTween.To(() => joint.zDrive.positionDamper, v =>
        {
            var tmp = joint.zDrive;
            tmp.positionDamper = v;
            joint.zDrive = tmp;
        }, endValue, duration).SetTarget(joint).SetUpdate(UpdateType.Fixed);
    public static TweenerCore<float, float, FloatOptions> DOZDrive_PositionSpring(this ConfigurableJoint joint, float endValue, float duration)
        => DOTween.To(() => joint.zDrive.positionSpring, v =>
        {
            var tmp = joint.zDrive;
            tmp.positionSpring = v;
            joint.zDrive = tmp;
        }, endValue, duration).SetTarget(joint).SetUpdate(UpdateType.Fixed);

}
