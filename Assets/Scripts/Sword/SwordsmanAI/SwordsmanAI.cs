using MarkusSecundus.Util;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class SwordsmanAI : MonoBehaviour
{
    public NavMeshObstacle Target;
    [SerializeField] SwordsmanAssembly SwordsmanAssembly;
    [SerializeField] InputSimulator Input;
    NavMeshAgent agent;
    SwordMovementMode_PlayRecord recordPlayer;
    SwordsmanMovement Swordsman => SwordsmanAssembly.Player;
    SwordMovement Sword => SwordsmanAssembly.Sword;

    void Start()
    {
        SetupNavmeshAgent();
        SetupSwordRecordPlayer();
    }
    void Update()
    {
        SetNavmeshTarget();
        SetSwordsmanMoveInput();
    }

    private void FixedUpdate()
    {
        SetSwordRecord();
    }


    #region Navigation

    [System.Serializable] public struct NavigationConfigurator
    {
        public float SidewaysRotationMultiplier;
        public float AgentSync;
        public float MelleeReachMultiplier;
        public AnimationCurve RotationAccuracyByDistance;
        public static readonly NavigationConfigurator Default = new NavigationConfigurator { SidewaysRotationMultiplier = 1f, AgentSync = 0.9f, MelleeReachMultiplier = 1.1f };
    }
    public NavigationConfigurator Navigation = NavigationConfigurator.Default;

    void SetupNavmeshAgent()
    {
        agent = GetComponentInChildren<NavMeshAgent>();
        agent.DisableAllUpdates();
        agent.Warp(agent.transform.position);
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
        var melleeDistance = (Target.size.xz().magnitude * 0.5f + agent.radius) * Navigation.MelleeReachMultiplier;
        var distanceRatio = distanceToTarget / melleeDistance;

        var deltaPosition = agent.nextPosition - tr.position;


        var forward = ClampAxis(deltaPosition.Dot(Swordsman.MovementDirectionBases.WalkForwardBackwardBase));
        var sideways = ClampAxis(deltaPosition.Dot(Swordsman.MovementDirectionBases.StrafeLeftRightBase));
        
        var rotate = ClampAxis(directionToTarget.Dot(Swordsman.MovementDirectionBases.StrafeLeftRightBase));
        var rotateSideways = ClampAxis(sideways * Navigation.SidewaysRotationMultiplier);
        rotate = ClampAxis(Mathf.Lerp(rotateSideways, rotate, Navigation.RotationAccuracyByDistance.Evaluate(distanceRatio)));

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
    #endregion

    #region Sword


    [System.Serializable]
    public class SwordConfig
    {
        [SerializeField] public SwordRecordUsecase Usecase = SwordRecordUsecase.Idle;
        [SerializeField] public float PlaySpeed = 1f;
        [SerializeField] public float DistanceToIdle = 5f;
        [SerializeField] public SerializableDictionary<SwordRecordUsecase, TextAsset[]> Records;
    }

    public SwordConfig SwordControl = new SwordConfig();

    private static readonly DefaultValDict<TextAsset, SwordMovementRecord> recordCache = new DefaultValDict<TextAsset, SwordMovementRecord>(t => JsonConvert.DeserializeObject<SwordMovementRecord>(t.text));
    void SetupSwordRecordPlayer()
    {
        var recordsList = SwordControl.Records.Values.Select(
            kv => new KeyValuePair<SwordRecordUsecase, SwordMovementRecord[]>(kv.Key,
                kv.Value.Select(t => recordCache[t]).ToArray()
            )
        ).ToDictionary();
        foreach(var (usecase, arr) in recordsList) Debug.Log($"Loaded {arr.Length} records for section {usecase}", this);
        
        recordPlayer = new SwordMovementMode_PlayRecord { Records = recordsList};
        recordPlayer.Init(Sword);

        Sword.Modes = new ScriptSubmodulesContainer<KeyCode, SwordMovement.Submodule, ISwordMovement> { Default = recordPlayer };
    }

    void SetSwordRecord()
    {
        recordPlayer.PlaySpeed = SwordControl.PlaySpeed;
        if (Swordsman.transform.position.Distance(Target.transform.position) > SwordControl.DistanceToIdle)
            recordPlayer.CurrentUsecase = SwordControl.Usecase = SwordRecordUsecase.Idle;
        else
            recordPlayer.CurrentUsecase = SwordControl.Usecase = SwordRecordUsecase.Generic;
    } 
    #endregion
}
