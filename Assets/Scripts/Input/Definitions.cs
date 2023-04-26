using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


namespace MarkusSecundus.PhysicsSwordfight.Input
{

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
    public static class InputAxisExtensions
    {
        public static string Name(this InputAxis axis)
        {
            return axis.ToString();
        }
    }
}