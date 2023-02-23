using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActiveRagdollManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }



    private void DisableChildCollisions()
    {
        var toDisable = GetComponentsInChildren<Collider>();

        foreach (var pair in toDisable.AllCombinations())
            Physics.IgnoreCollision(pair.First, pair.Second, true);
    }
}
