using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SwordMovementModesContainer
{
    public SwordMovementMode_Basic Basic;
    public SwordMovementMode_Block Block;
    public KeyCode BlockKey = KeyCode.LeftShift;

    public Dictionary<KeyCode, IScriptSubmodule<SwordMovement>> MakeMap() => new Dictionary<KeyCode, IScriptSubmodule<SwordMovement>>
    {
        [default] = Basic,
        [BlockKey] = Block
    };
}
