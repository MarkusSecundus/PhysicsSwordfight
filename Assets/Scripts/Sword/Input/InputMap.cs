using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class InputMap
{
    private InputMap() { }
    public enum Key
    {
        WalkForward, WalkBackward, StrafeLeft, StrafeRight, LookLeft, LookRight, BlockMode, StabMode, 
    }
    public abstract KeyCode this[Key k] { get;set; }

    public abstract void Save();

    private class Impl : InputMap
    {
        private Dictionary<Key, KeyCode> values = new Dictionary<Key, KeyCode>();
        public override KeyCode this[Key k] { get => values.GetValueOrDefault(k); set => values[k] = value; }

        public override void Save()
        {
        }
    }
}



