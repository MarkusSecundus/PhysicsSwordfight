using MarkusSecundus.PhysicsSwordfight.Input;
using MarkusSecundus.PhysicsSwordfight.Submodules;
using MarkusSecundus.PhysicsSwordfight.Sword;
using MarkusSecundus.PhysicsSwordfight.Sword.Recording;
using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using MarkusSecundus.PhysicsSwordfight.Utils.Navigation;
using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using MarkusSecundus.PhysicsSwordfight.Utils.Serialization;
using MarkusSecundus.Utils;
using MarkusSecundus.Utils.Datastructs;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace MarkusSecundus.PhysicsSwordfight.Sword.AI
{
    /// <summary>
    /// Simple AI algorithm that navigates the swordsman through terrain using <see cref="NavMesh"/> and lets him attack specified target by playing prerecorded sword movements.
    /// 
    /// <para>
    /// Requires <see cref="InputSimulator"/> and <see cref="SwordsmanAssembly"/> to be present in the same gameobject and <see cref="NavMeshAgent"/> corresponding to the swordsman's body somewhere in child objects.
    /// </para>
    /// </summary>
    [RequireComponent(typeof(InputSimulator)), RequireComponent(typeof(SwordsmanAssembly))]
    public class SwordsmanAI : MonoBehaviour
    {
        /// <summary>
        /// Target to attack
        /// </summary>
        [Tooltip("Target to attack")]
        public NavMeshObstacle Target;

        SwordsmanAssembly SwordsmanAssembly;
        InputSimulator Input;
        NavMeshAgent agent;
        SwordMovementMode_PlayRecord recordPlayer;
        SwordsmanMovement Swordsman => SwordsmanAssembly.Player;
        SwordMovement Sword => SwordsmanAssembly.Sword;

        void Start()
        {
            SwordsmanAssembly = GetComponent<SwordsmanAssembly>();
            Input = GetComponent<InputSimulator>();

            SetupNavmeshAgent();
            SetupSwordRecordPlayer();
        }
        void Update()
        {
            SetNavmeshTarget();
            SetSwordsmanMoveInput();
        }

        void FixedUpdate()
        {
            SetSwordRecord();
        }


        #region Navigation

        /// <summary>
        /// Config for swordsman's movement through terrain
        /// </summary>
        [Tooltip("Config for swordsman's movement through terrain")]
        [System.Serializable]
        public struct NavigationConfigurator
        {
            /// <summary>
            /// How fast to rotate sideways
            /// </summary>
            [Tooltip("How fast to rotate sideways")]
            public float SidewaysRotationMultiplier;
            /// <summary>
            /// Number in interval [0;1] - how much does the <see cref="NavMeshAgent"/> get synced back into current Transform position
            /// </summary>
            [Tooltip("Number in interval [0;1] - how much does the NavMeshAgent get synced back into current Transform position")]
            [Range(0f,1f)]
            public float AgentSync;
            /// <summary>
            /// How far from the swordsman to start playing attacking sword moves. Ratio relative to the sum of <c>this</c> and <see cref="Target"/>'s radius.
            /// </summary>
            [Tooltip("How far from the swordsman to start playing attacking sword moves. Ratio relative to the sum of this and Target's radius")]
            public float MelleeReachMultiplier;
            /// <summary>
            /// How accurately swordsman's rotation should face the <see cref="Target"/> depending on his distance. Not totally accurate when distance is big looks better.
            /// </summary>
            [Tooltip("How accurately swordsman's rotation should face the Target depending on his distance. Not totally accurate when distance is big looks better")]
            public AnimationCurve RotationAccuracyByDistance;
            /// <summary>
            /// Default values for editor
            /// </summary>
            public static readonly NavigationConfigurator Default = new NavigationConfigurator { SidewaysRotationMultiplier = 1f, AgentSync = 0.9f, MelleeReachMultiplier = 1.1f, RotationAccuracyByDistance = NumericConstants.AnimationCurve01 };
        }
        /// <summary>
        /// Config for swordsman's movement through terrain
        /// </summary>
        [Tooltip("Config for swordsman's movement through terrain")]
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

            if (distanceToTarget > melleeDistance)
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
        static float ClampAxis(float f) => Mathf.Clamp(f, -1f, 1f);
        #endregion

        #region Sword


        /// <summary>
        /// Config for swordsman's attacks
        /// </summary>
        [Tooltip("Config for swordsman's attacks")]
        [System.Serializable]
        public class SwordConfig
        {
            /// <summary>
            /// How fast the sword moves should be replayed
            /// </summary>
            [Tooltip("How fast the sword moves should be replayed")]
            [SerializeField] public float PlaySpeed = 1f;
            /// <summary>
            /// Distance from <see cref="Target"/> at which swordsman plays Idle moves
            /// </summary>
            [Tooltip("Distance from Target at which swordsman plays Idle moves")]
            [SerializeField] public float DistanceToIdle = 5f;
            /// <summary>
            /// Records (files containing JSON of <see cref="SwordMovementRecord"/>) to be played for each state.
            /// </summary>
            [Tooltip("Records (files containing JSON of SwordMovementRecord) to be played for each state")]
            [SerializeField] public SerializableDictionary<SwordRecordUsecase, TextAsset[]> Records;
        }

        /// <summary>
        /// Config for swordsman's attacks
        /// </summary>
        [Tooltip("Config for swordsman's attacks")]
        public SwordConfig SwordControl = new SwordConfig();

        static readonly DefaultValDict<TextAsset, SwordMovementRecord> recordCache = new DefaultValDict<TextAsset, SwordMovementRecord>(t => JsonConvert.DeserializeObject<SwordMovementRecord>(t.text));
        void SetupSwordRecordPlayer()
        {
            var recordsList = new Dictionary<SwordRecordUsecase, SwordMovementRecord[]>(SwordControl.Records.Values.Select(
                kv => new KeyValuePair<SwordRecordUsecase, SwordMovementRecord[]>(kv.Key,
                    kv.Value.Select(t => recordCache[t]).ToArray()
                )
            ));
            foreach (var (usecase, arr) in recordsList) Debug.Log($"Loaded {arr.Length} records for section {usecase}", this);

            recordPlayer = new SwordMovementMode_PlayRecord { Records = recordsList };
            recordPlayer.Init(Sword);

            Sword.Modes = new ScriptSubmodulesContainer<KeyCode, SwordMovement.Module, ISwordMovement> { Default = recordPlayer };
        }

        void SetSwordRecord()
        {
            recordPlayer.PlaySpeed = SwordControl.PlaySpeed;
            if (Swordsman.transform.position.Distance(Target.transform.position) > SwordControl.DistanceToIdle)
                recordPlayer.CurrentUsecase = SwordRecordUsecase.Idle;
            else
                recordPlayer.CurrentUsecase = SwordRecordUsecase.Attack;
        }
        #endregion
    }
}