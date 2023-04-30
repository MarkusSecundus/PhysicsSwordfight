using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Input
{
    /// <summary>
    /// Placeholder implementation of <see cref="ISwordInput"/> that returns <c>default</c> values for everything.
    /// </summary>
    public class NullSwordInput : MonoBehaviour, ISwordInput
    {
        /// <inheritdoc/>
        public float GetAxisRaw(InputAxis axis) => 0f;
        /// <inheritdoc/>
        public float GetAxis(InputAxis axis) => 0f;
        /// <inheritdoc/>
        public Ray? GetInputRay() => null;
        /// <inheritdoc/>
        public bool GetKey(KeyCode code) => false;
        /// <inheritdoc/>
        public bool GetKeyDown(KeyCode code) => false;
        /// <inheritdoc/>
        public bool GetKeyUp(KeyCode code) => false;
    }
}