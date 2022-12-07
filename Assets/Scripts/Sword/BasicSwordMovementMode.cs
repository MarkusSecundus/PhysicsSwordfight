using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasicSwordMovementMode : ISwordMovementMode
{
    public BasicSwordMovementMode(SwordMovement script) : base(script){}

    public override void FixedUpdate(float delta)
    {
        base.FixedUpdate(delta);
    
        
    }
}
