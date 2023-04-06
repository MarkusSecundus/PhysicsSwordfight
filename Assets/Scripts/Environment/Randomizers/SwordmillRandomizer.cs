using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SwordmillAssembly))]
public class SwordmillRandomizer : MonoBehaviour, IRandomizer
{
    public Interval<int> SwordsCount = new Interval<int>(1, 3);
    public Interval<float> Damper = new Interval<float>(2f, 10f), Spring = new Interval<float>(10f, 30f);
    public Interval<bool> ShouldFreezePrimaryRotationAngle = new Interval<bool>(false, true);
    public Interval<bool> ShouldFreezeSecondaryRotationAngles = new Interval<bool>(false, true);

    public Interval<int> RotorPathSegmentsCount = new Interval<int>(0, 4);
    public Interval<Vector3> RotorPathSegmentsArea = new Interval<Vector3>(new Vector3(0, -0.2f, 0), new Vector3(0, 0.2f, 0));
    public Interval<float> RotorPathSegmentDuration = new Interval<float>(1f, 6f);

    public void Randomize(System.Random random)
    {
        var assembly = GetComponent<SwordmillAssembly>();
        assembly.SwordsCount = random.Next(SwordsCount);
        var rotor = assembly.Rotor;

        var drive = rotor.slerpDrive;
        drive.positionSpring = random.Next(Spring);
        drive.positionDamper = random.Next(Damper);
        assembly.Rotor.slerpDrive = drive;


        rotor.angularYMotion = random.Next(ShouldFreezePrimaryRotationAngle)?ConfigurableJointMotion.Locked: ConfigurableJointMotion.Free;
        rotor.angularXMotion = rotor.angularZMotion = random.Next(ShouldFreezeSecondaryRotationAngles) ? ConfigurableJointMotion.Locked : ConfigurableJointMotion.Free;

        var manager = GetComponent<SwordmillBehavior>();
        if (manager.IsNil()) return;

        manager.Movements = new SwordmillBehavior.MovementStep[random.Next(RotorPathSegmentsCount)];
        for(int t = 0; t < manager.Movements.Length; ++t)
        {
            manager.Movements[t].Offset = random.Next(RotorPathSegmentsArea);
            manager.Movements[t].Duration = random.Next(RotorPathSegmentDuration);
        }
    }
}
