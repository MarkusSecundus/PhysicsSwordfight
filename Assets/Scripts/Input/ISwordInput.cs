using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Input
{

    public interface ISwordInput
    {
        public abstract bool GetKey(KeyCode code);
        public abstract bool GetKeyUp(KeyCode code);
        public abstract bool GetKeyDown(KeyCode code);

        public abstract float GetAxis(InputAxis axis);
        public abstract float GetAxisRaw(InputAxis axis);

        public abstract Ray? GetInputRay();

        public static ISwordInput Get(GameObject self) => self.GetComponentInParent<ISwordInput>();
    }
}