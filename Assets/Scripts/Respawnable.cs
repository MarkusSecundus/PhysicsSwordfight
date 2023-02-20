using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Respawnable : MonoBehaviour
{
    public Checkpoint LastCheckpoint;

    public UnityEvent OnDie;

    // Start is called before the first frame update
    void Start()
    {
        
    }



    public void Die()
    {
        this.transform.position = LastCheckpoint.SpawnPoint.position;
        this.transform.rotation = LastCheckpoint.SpawnPoint.rotation;
        OnDie.Invoke();
        LastCheckpoint.OnPlayerRespawn.Invoke();
    }
}
