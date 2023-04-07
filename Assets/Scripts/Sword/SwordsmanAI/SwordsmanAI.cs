using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SwordsmanAI : MonoBehaviour
{
    public NavMeshObstacle Target;
    [SerializeField] SwordsmanAssembly SwordsmanAssembly;
    [SerializeField] InputSimulator Input;
    NavMeshAgent agent;

    SwordsmanMovement Swordsman => SwordsmanAssembly.Player;
    SwordMovement Sword => SwordsmanAssembly.Sword;

    [System.Serializable]public struct TweaksList
    {
        public float SidewaysRotationMultiplier;
        public float AgentSync;
        public float MelleeReachMultiplier;
        public AnimationCurve RotationAccuracyByDistance;
        public static readonly TweaksList Default = new TweaksList { SidewaysRotationMultiplier = 1f, AgentSync = 0.9f , MelleeReachMultiplier = 1.1f};
    }
    public TweaksList Tweaks = TweaksList.Default;

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
        if (Target.IsNotNil())
        {
            agent.isStopped = false;
            agent.SetDestination(Target.transform.position);
        }
        else
            agent.isStopped = true;
    }

    void SetSwordsmanMoveInput()
    {
        if (Target.IsNil()) return;

        var tr = Swordsman.transform;
        var directionToTarget = Target.transform.position - tr.position;
        var distanceToTarget = directionToTarget.magnitude;
        var melleeDistance = (Target.size.xz().magnitude * 0.5f + agent.radius) * Tweaks.MelleeReachMultiplier;
        var distanceRatio = distanceToTarget / melleeDistance;

        var deltaPosition = agent.nextPosition - tr.position;


        var forward = ClampAxis(deltaPosition.Dot(Swordsman.MovementDirectionBases.WalkForwardBackwardBase));
        var sideways = ClampAxis(deltaPosition.Dot(Swordsman.MovementDirectionBases.StrafeLeftRightBase));
        
        var rotate = ClampAxis(directionToTarget.Dot(Swordsman.MovementDirectionBases.StrafeLeftRightBase));
        var rotateSideways = ClampAxis(sideways * Tweaks.SidewaysRotationMultiplier);
        rotate = ClampAxis(Mathf.Lerp(rotateSideways, rotate, Tweaks.RotationAccuracyByDistance.Evaluate(distanceRatio)));

        if (distanceToTarget> melleeDistance)
        {
            Input.SetAxisValue(Swordsman.Mapping.WalkForwardBackward, forward);
            Input.SetAxisValue(Swordsman.Mapping.StrafeLeftRight, sideways);
            Input.SetAxisValue(Swordsman.Mapping.RotateLeftRight, rotate);

            agent.nextPosition = tr.position + 0.9f * deltaPosition;
        }
        else
        {
            Input.SetAxisValue(Swordsman.Mapping.WalkForwardBackward, 0f);
            Input.SetAxisValue(Swordsman.Mapping.StrafeLeftRight, 0f);
            Input.SetAxisValue(Swordsman.Mapping.RotateLeftRight, rotate);

            agent.nextPosition = tr.position;
        }
        
    }

    private static float ClampAxis(float f) => Mathf.Clamp(f, -1f, 1f);
}
