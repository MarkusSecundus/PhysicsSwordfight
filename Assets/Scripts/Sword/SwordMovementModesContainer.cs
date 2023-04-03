using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Submodule = IScriptSubmodule<SwordMovement>;

[System.Serializable]
public class SwordMovementModesContainer
{
    public SwordMovementMode_Basic Basic;

    public SwordMovementMode_Block Block;
    public KeyCode BlockKey = KeyCode.LeftShift;

    public SwordMovementMode_Stabbing Stabbing;
    public KeyCode StabbingKey = KeyCode.LeftControl;

    public SwordMovementModesContainer(SwordMovement script)
    {
        this.InitFields<Submodule>(new object[] { script });
    }

    public Dictionary<KeyCode, Submodule> MakeMap()
    {
        var ret = new Dictionary<KeyCode, Submodule>();
        var self = this.GetType();
        var fields = self.GetFieldsOfType<Submodule>().ToArray();
        foreach (var f in fields)
        {
            ret.Add((self.GetField(f.Name + "Key")?.GetValue(this) as KeyCode?) ?? default, (Submodule)f.GetValue(this));
        }
        return ret;
    }
}
