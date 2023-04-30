using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


namespace MarkusSecundus.PhysicsSwordfight.Input
{
    /// <summary>
    /// Input axes defined in the project. To not waste hours of debugging with Unity's default stringly typed nonsense.
    /// </summary>
    public enum InputAxis
    {
        None = 0,
        Horizontal = 1,
        Vertical = 2,
        HorizontalSecondary = 3,
        HorizontalQE = 6,
        MouseX = 4,
        MouseY = 5
    }
}