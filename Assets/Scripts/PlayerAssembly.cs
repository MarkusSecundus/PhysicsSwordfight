using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAssembly : MonoBehaviour
{
    public PlayerMovement Player;
    public SwordMovement Sword;

    public bool IsMainPlayer = false;
    void Start()
    {
        if (IsMainPlayer)
        {

        }
    }
}
