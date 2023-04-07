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
        SetNavmeshTarget();
        SetSwordsmanMoveInput();
    }

    void SetNavmeshTarget()
    {
        agent.SetDestination(Target.position);
    }

    void SetSwordsmanMoveInput()
    {
        var tr = Swordsman.transform;
        var deltaPosition = agent.nextPosition - tr.position;

        var forward = deltaPosition.Dot(tr.forward);
        var right = deltaPosition.Dot(tr.right);

        Input.SetAxisValue(Swordsman.Mapping.WalkForwardBackward, forward);
        Input.SetAxisValue(Swordsman.Mapping.StrafeLeftRight, right);
        Input.SetAxisValue(Swordsman.Mapping.RotateLeftRight, right);

        agent.nextPosition = tr.position + 0.9f * deltaPosition;
    }
}
