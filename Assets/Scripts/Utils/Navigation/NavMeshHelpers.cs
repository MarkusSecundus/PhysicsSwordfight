using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Navigation
{
    public static class NavMeshHelpers
    {
        public static void DisableAllUpdates(this NavMeshAgent self)
        {
            self.updatePosition = false;
            self.updateRotation = false;
            self.updateUpAxis = false;
        }
    }
}
