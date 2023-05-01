using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Automatization
{
    /// <summary>
    /// Composite implementation of <see cref="IPointsSupplier"/> that carries a list of subinstances and gets points from them.
    /// </summary>
    public class AggregatePointsSupplier : IPointsSupplier
    {
        /// <summary>
        /// Subproviders of this composite points supplier.
        /// </summary>
        public IPointsSupplier[] Values;


        private IPointsSupplier GetRandomSupplier(System.Random rand) => Values[rand.Next(0, Values.Length)];
        /// <inheritdoc/>
        public override Vector3 GetRandomPoint(System.Random rand) => GetRandomSupplier(rand).GetRandomPoint(rand);

        /// <inheritdoc/>
        public override Vector3 GetRandomPointInVolume(System.Random rand) => GetRandomSupplier(rand).GetRandomPointInVolume(rand);

        /// <inheritdoc/>
        public override IEnumerable<Vector3> IteratePoints()
            => Values.SelectMany(v => v.IteratePoints());
    }
}