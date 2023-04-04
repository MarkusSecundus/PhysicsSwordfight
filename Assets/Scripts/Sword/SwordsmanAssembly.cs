using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordsmanAssembly : MonoBehaviour
{
    public SwordsmanMovement Player;
    public SwordMovement Sword;

    public bool IsMainPlayer = false;
    void Start()
    {
        if (IsMainPlayer)
        {

        }
    }

    public void DestroyTheSwordsman()
    {
        Sword.GetComponent<Rigidbody>().useGravity = true;
        Sword.transform.SetParent(null);
        Destroy(Sword.Joint);
        Destroy(Sword);
        Destroy(gameObject);
    }
}
