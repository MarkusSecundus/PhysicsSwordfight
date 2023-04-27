using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Input
{

    public class NullSwordInput : MonoBehaviour, ISwordInput
    {
        public float GetAxisRaw(InputAxis axis) => 0f;
        public float GetAxis(InputAxis axis) => 0f;
        public Ray? GetInputRay() => null;
        public bool GetKey(KeyCode code) => false;
        public bool GetKeyDown(KeyCode code) => false;
        public bool GetKeyUp(KeyCode code) => false;
    }
}