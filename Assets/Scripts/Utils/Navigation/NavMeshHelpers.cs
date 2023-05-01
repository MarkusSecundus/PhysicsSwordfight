using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Navigation
{
    /// <summary>
    /// Static class containing convenience extensions methods for <see cref="NavMesh"/>-related stuff
    /// </summary>
    public static class NavMeshHelpers
    {
        /// <summary>
        /// Set the navmesh agent to not write to its <see cref="Transform"/> in any manner.
        /// </summary>
        /// <param name="self">Navmesh agent</param>
        public static void DisableAllUpdates(this NavMeshAgent self)
        {
            self.updatePosition = false;
            self.updateRotation = false;
            self.updateUpAxis = false;
        }
    }
}
