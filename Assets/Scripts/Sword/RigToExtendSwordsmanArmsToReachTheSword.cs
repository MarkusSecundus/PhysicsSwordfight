using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

public class RigToExtendSwordsmanArmsToReachTheSword : RigConstraint<RigToExtendSwordsmanArmsToReachTheSword.Job, RigToExtendSwordsmanArmsToReachTheSword.Data, RigToExtendSwordsmanArmsToReachTheSword.Binder>
{
    //[BurstCompile]
    public struct Job : IWeightedAnimationJob
    {
        public ReadWriteTransformHandle constrained;
        public ReadOnlyTransformHandle source;

        public FloatProperty jobWeight { get; set; }
        public void ProcessRootMotion(AnimationStream stream) { }

        public void ProcessAnimation(AnimationStream stream)
        {
            float weight = jobWeight.Get(stream);
            if (weight <= 0f) return;

            constrained.SetPosition(
                stream,
                Vector3.Lerp(constrained.GetPosition(stream), -source.GetPosition(stream), weight)
            );
        }

    }
    [System.Serializable]
    public struct Data : IAnimationJobData
    {
        public Transform constrainedObject;
        [SyncSceneToStream] public Transform sourceObject;

        public bool IsValid() => !(constrainedObject == null || sourceObject == null);
        public void SetDefaultValues() => (constrainedObject, sourceObject) = (null, null);
    }

    public class Binder : AnimationJobBinder<Job, Data>
    {
        public override Job Create(Animator animator, ref Data data, Component component)
            => new Job
            {
                constrained = ReadWriteTransformHandle.Bind(animator, data.constrainedObject),
                source = ReadOnlyTransformHandle.Bind(animator, data.sourceObject)
            };

        public override void Destroy(Job job){}
    }
}