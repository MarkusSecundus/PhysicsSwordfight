using MarkusSecundus.PhysicsSwordfight.Automatization;
using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using MarkusSecundus.PhysicsSwordfight.Utils.Randomness;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Environment.Randomization
{
    /// <summary>
    /// Randomizes some parameters of <see cref="SwordmillAssembly"/> and <see cref="SwordmillBehavior"/> (if present)
    /// </summary>
    [RequireComponent(typeof(SwordmillAssembly))]
    public class SwordmillRandomizer : MonoBehaviour, IRandomizer
    {
        /// <summary>
        /// Number of swordmill's swords
        /// </summary>
        public Interval<int> SwordsCount = new Interval<int>(1, 3);
        /// <summary>
        /// Damper of the <see cref="ConfigurableJoint"/> that's in the sword game object
        /// </summary>
        public Interval<float> Damper = new Interval<float>(2f, 10f), Spring = new Interval<float>(10f, 30f);
        /// <summary>
        /// Whether the Y angular movement should be frozen
        /// </summary>
        public Interval<bool> ShouldFreezePrimaryRotationAngle = new Interval<bool>(false, true);
        /// <summary>
        /// Whether the X and Z angular movement should be frozen
        /// </summary>
        public Interval<bool> ShouldFreezeSecondaryRotationAngles = new Interval<bool>(false, true);

        /// <summary>
        /// How many segments should have the rotor path of  <see cref="SwordmillBehavior"/>
        /// </summary>
        public Interval<int> RotorPathSegmentsCount = new Interval<int>(0, 4);
        /// <summary>
        /// Range in which all the <see cref="SwordmillBehavior.MovementStep.Offset"/>s will be chosen.
        /// </summary>
        public Interval<Vector3> RotorPathSegmentsArea = new Interval<Vector3>(new Vector3(0, -0.2f, 0), new Vector3(0, 0.2f, 0));
        /// <summary>
        /// Duration of one rotor path segment
        /// </summary>
        public Interval<float> RotorPathSegmentDuration = new Interval<float>(1f, 6f);

        /// <inheritdoc/>
        public void Randomize(System.Random random)
        {
            var assembly = GetComponent<SwordmillAssembly>();
            assembly.SwordsCount = random.Next(SwordsCount);
            var rotor = assembly.Rotor;

            var drive = rotor.slerpDrive;
            drive.positionSpring = random.Next(Spring);
            drive.positionDamper = random.Next(Damper);
            assembly.Rotor.slerpDrive = drive;


            rotor.angularYMotion = random.Next(ShouldFreezePrimaryRotationAngle) ? ConfigurableJointMotion.Locked : ConfigurableJointMotion.Free;
            rotor.angularXMotion = rotor.angularZMotion = random.Next(ShouldFreezeSecondaryRotationAngles) ? ConfigurableJointMotion.Locked : ConfigurableJointMotion.Free;

            var manager = GetComponent<SwordmillBehavior>();
            if (manager.IsNil()) return;

            manager.Movements = new SwordmillBehavior.MovementStep[random.Next(RotorPathSegmentsCount)];
            for (int t = 0; t < manager.Movements.Length; ++t)
            {
                manager.Movements[t].Offset = random.Next(RotorPathSegmentsArea);
                manager.Movements[t].Duration = random.Next(RotorPathSegmentDuration);
            }
        }
    }
}