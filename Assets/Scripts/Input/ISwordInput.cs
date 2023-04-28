using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Input
{

    public interface ISwordInput
    {
        public bool GetKey(KeyCode code);
        public bool GetKeyUp(KeyCode code);
        public bool GetKeyDown(KeyCode code);

        public float GetAxis(InputAxis axis);
        public float GetAxisRaw(InputAxis axis);

        public Ray? GetInputRay();

        public static ISwordInput Get(GameObject self) => self.GetComponentInParent<ISwordInput>();
    }
}