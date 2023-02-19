using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public enum InputAxis
{
    Horizontal,
    Vertical,
    HorizontalSecondary,
    MouseX,
    MouseY
}
public static class InputAxisExtensions
{
    public static string Name(this InputAxis axis)
    {
        return axis.ToString();
    }
}
