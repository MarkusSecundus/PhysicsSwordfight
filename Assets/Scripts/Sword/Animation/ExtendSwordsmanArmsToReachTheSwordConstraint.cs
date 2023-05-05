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
    public class ExtendSwordsmanArmsToReachTheSwordConstraint : RigConstraint<ExtendSwordsmanArmsToReachTheSwordConstraint.Job, ExtendSwordsmanArmsToReachTheSwordConstraint.Data, ExtendSwordsmanArmsToReachTheSwordConstraint.Binder>
    {
        [BurstCompile]
        public struct Job : IWeightedAnimationJob
        {
            /// <summary>
            /// Bones of the arm
            /// </summary>
            public NativeArray<ReadWriteTransformHandle> ArmSegments;
            /// <summary>
            /// Point in worldspace that should be made reachable for the arm
            /// </summary>
            public ReadOnlyTransformHandle TargetPoint;
            /// <summary>
            /// How much this constraint should overshoot - <c>1f</c> will be just reachable (which is still not totally stable for IK constraints).
            /// </summary>
            public float Multiplier;

            public FloatProperty jobWeight { get; set; }
            public void ProcessRootMotion(AnimationStream stream) { }

            public void ProcessAnimation(AnimationStream stream)
            {
                float weight = jobWeight.Get(stream);
                if (weight <= 0f) return;

                var rootPos = ArmSegments[0].GetPosition(stream);
                var targetPos = TargetPoint.GetPosition(stream);
                var targetLength = targetPos.Distance(rootPos) * Multiplier * weight;


                float lengthSum = 0f;
                var lastPos = rootPos;
                for (int t = 1; t < ArmSegments.Length; ++t)
                {
                    var currentPos = ArmSegments[t].GetPosition(stream);
                    lengthSum += currentPos.Distance(lastPos);
                    lastPos = currentPos;
                }
                if (lengthSum >= targetLength) return;

                var ratio = targetLength / lengthSum;

                //TODO: make it correctly take in consideration the transform's scale
                for (int t = 1; t < ArmSegments.Length; ++t)
                {
                    var currentPos = ArmSegments[t].GetLocalPosition(stream);
                    ArmSegments[t].SetLocalPosition(stream, currentPos * ratio);
                }
            }

        }
        [System.Serializable]
        public struct Data : IAnimationJobData
        {
            /// <summary>
            /// Bones of the arm
            /// </summary>
            public TransformChain Target;
            /// <summary>
            /// Point in worldspace that should be made reachable for the arm
            /// </summary>
            [SyncSceneToStream] public Transform SourceObject;
            /// <summary>
            /// How much this constraint should overshoot - <c>1f</c> will be just reachable (which is still not totally stable for IK constraints).
            /// </summary>
            public float Multiplier;

            public bool IsValid() => Target.IsValid() && SourceObject.IsNotNil();
            public void SetDefaultValues() => (Target, SourceObject, Multiplier) = (default, null, 1.1f);
        }

        public class Binder : AnimationJobBinder<Job, Data>
        {
            public override Job Create(Animator animator, ref Data data, Component component)
            {
                var armSegmentsManaged = data.Target.ToArray();
                var armSegments = new NativeArray<ReadWriteTransformHandle>(armSegmentsManaged.Length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                for (int t = 0; t < armSegments.Length; ++t)
                    armSegments[t] = ReadWriteTransformHandle.Bind(animator, armSegmentsManaged[t]);
                return new Job
                {
                    ArmSegments = armSegments,
                    TargetPoint = ReadOnlyTransformHandle.Bind(animator, data.SourceObject),
                    Multiplier = data.Multiplier
                };
            }

            public override void Destroy(Job job)
            {
                job.ArmSegments.Dispose();
            }
        }
    }
}