using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AggregatePointsSupplier : IPointsSupplier
{
    public IPointsSupplier[] Values;


    private IPointsSupplier GetRandomSupplier(System.Random rand) => Values[rand.Next(0, Values.Length)];
    public override Vector3 GetRandomPoint(System.Random rand) => GetRandomSupplier(rand).GetRandomPoint(rand);

    public override Vector3 GetRandomPointInVolume(System.Random rand) => GetRandomSupplier(rand).GetRandomPointInVolume(rand);

    public override IEnumerable<Vector3> IteratePoints()
        => Values.SelectMany(v => v.IteratePoints());
}