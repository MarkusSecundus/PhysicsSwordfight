using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Input
{
    /// <summary>
    /// Provider of keyboard and mouse user input.
    /// 
    /// For basic implementation that reads input from <see cref="UnityEngine.Input"/> see <see cref="BasicSwordInput"/>.
    /// </summary>
    public interface ISwordInput
    {
        /// <summary>
        /// Check if the key is currently being pressed.
        /// </summary>
        /// <param name="code">Key to be checked</param>
        /// <returns><c>true</c> IFF the key is currently being pressed.</returns>
        public bool GetKey(KeyCode code);
        /// <summary>
        /// Check if the key was pressed down in the current frame.
        /// </summary>
        /// <param name="code">Key to be checked</param>
        /// <returns><c>true</c> IFF the key was pressed down in the current frame</returns>
        public bool GetKeyUp(KeyCode code);
        /// <summary>
        /// Check if the keypress was lifted up in the current frame.
        /// </summary>
        /// <param name="code">Key to be checked</param>
        /// <returns><c>true</c> IFF pressed state of the provided key ended in the current frame</returns>
        public bool GetKeyDown(KeyCode code);

        /// <summary>
        /// Get value in interval [-1;1] representing state of the provided input Axis.
        /// </summary>
        /// <param name="axis">Input axis to be checked</param>
        /// <returns>Value in interval [-1;1] representing state of the provided input Axis</returns>
        public float GetAxis(InputAxis axis);
        /// <summary>
        /// Get value in interval [-1;1] representing state of the provided input Axis, without interpolation.
        /// </summary>
        /// <param name="axis">Input axis to be checked</param>
        /// <returns>Value in interval [-1;1] representing state of the provided input Axis. No interpolation used.</returns>
        public float GetAxisRaw(InputAxis axis);

        /// <summary>
        /// Get ray in worldspace representing the current cursor position. (Current position of the cursor on the screen shot from the screen's camera).
        /// </summary>
        /// <returns>Ray in worldspace representing the current cursor position.</returns>
        public Ray? GetInputRay();

        /// <summary>
        /// Finds the cannonical input provider for the provided gameobject - currently the one found by <see cref="Component.GetComponentInParent{T}()"/>
        /// </summary>
        /// <param name="self">Game object that's searching for its input provider.</param>
        /// <returns>Input provider to be used by the gameobject.</returns>
        public static ISwordInput Get(GameObject self) => self.GetComponentInParent<ISwordInput>();
    }
}