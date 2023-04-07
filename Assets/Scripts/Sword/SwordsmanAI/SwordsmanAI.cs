using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SwordsmanAI : MonoBehaviour
{
    public Transform Target;
    [SerializeField] SwordsmanAssembly SwordsmanAssembly;
    [SerializeField] InputSimulator Input;
    NavMeshAgent agent;

    SwordsmanMovement Swordsman => SwordsmanAssembly.Player;
    SwordMovement Sword => SwordsmanAssembly.Sword;

    void Start()
    {
        agent = GetComponentInChildren<NavMeshAgent>();
        agent.DisableAllUpdates();
    }


    void Update()
    {
        var tr = Swordsman.transform;
        var deltaPosition = agent.nextPosition - tr.position;


    }
}
