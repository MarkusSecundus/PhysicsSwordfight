using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Cosmetics
{
    public class CameraFollowPoint : MonoBehaviour
    {
        public Transform Target;
        private void LateUpdate()
        {
            if (Target.IsNotNil())
            {
                (transform.position, transform.rotation, transform.localScale) = (Target.position, Target.rotation, Target.localScale);
            }
        }
    }
}