using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DoTweenExtensions 
{
    public static TweenerCore<Vector3, Vector3, VectorOptions> DOConnectedAnchor(this ConfigurableJoint joint, Vector3 endValue, float duration) 
        => DOTween.To(() => joint.connectedAnchor, v => joint.connectedAnchor = v, endValue, duration).SetTarget(joint);
}
