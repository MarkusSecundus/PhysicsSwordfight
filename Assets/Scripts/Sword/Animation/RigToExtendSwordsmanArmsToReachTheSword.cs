using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

namespace MarkusSecundus.PhysicsSwordfight.Sword.Animation
{
    /// <summary>
    /// Procedural rig constraint that extends lengths of bone chain to ensure it can reach a set point. Supposed to be used as preprocess for IK constraint.
    /// </summary>
    public class RigToExtendSwordsmanArmsToReachTheSword : RigConstraint<RigToExtendSwordsmanArmsToReachTheSword.Job, RigToExtendSwordsmanArmsToReachTheSword.Data, RigToExtendSwordsmanArmsToReachTheSword.Binder>
    {
        [BurstCompile]
        public struct Job : IWeightedAnimationJob
        {
            public NativeArray<ReadWriteTransformHandle> armSegments;
            public ReadOnlyTransformHandle targetPoint;
            public float multiplier;

            public FloatProperty jobWeight { get; set; }
            public void ProcessRootMotion(AnimationStream stream) { }

            public void ProcessAnimation(AnimationStream stream)
            {
                float weight = jobWeight.Get(stream);
                if (weight <= 0f) return;

                var rootPos = armSegments[0].GetPosition(stream);
                var targetPos = targetPoint.GetPosition(stream);
                var targetLength = targetPos.Distance(rootPos) * multiplier * weight;


                float lengthSum = 0f;
                var lastPos = rootPos;
                for (int t = 1; t < armSegments.Length; ++t)
                {
                    var currentPos = armSegments[t].GetPosition(stream);
                    lengthSum += currentPos.Distance(lastPos);
                    lastPos = currentPos;
                }
                if (lengthSum >= targetLength) return;

                var ratio = targetLength / lengthSum;

                //TODO: make it correctly take in consideration the transform's scale
                for (int t = 1; t < armSegments.Length; ++t)
                {
                    var currentPos = armSegments[t].GetLocalPosition(stream);
                    armSegments[t].SetLocalPosition(stream, currentPos * ratio);
                }
            }

        }
        [System.Serializable]
        public struct Data : IAnimationJobData
        {
            public TransformChain target;
            [SyncSceneToStream] public Transform sourceObject;
            public float multiplier;

            public bool IsValid() => target.IsValid() && sourceObject.IsNotNil();
            public void SetDefaultValues() => (target, sourceObject, multiplier) = (default, null, 1.1f);
        }

        public class Binder : AnimationJobBinder<Job, Data>
        {
            public override Job Create(Animator animator, ref Data data, Component component)
            {
                var armSegmentsManaged = data.target.ToArray();
                var armSegments = new NativeArray<ReadWriteTransformHandle>(armSegmentsManaged.Length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                for (int t = 0; t < armSegments.Length; ++t)
                    armSegments[t] = ReadWriteTransformHandle.Bind(animator, armSegmentsManaged[t]);
                return new Job
                {
                    armSegments = armSegments,
                    targetPoint = ReadOnlyTransformHandle.Bind(animator, data.sourceObject),
                    multiplier = data.multiplier
                };
            }

            public override void Destroy(Job job)
            {
                job.armSegments.Dispose();
            }
        }
    }
}