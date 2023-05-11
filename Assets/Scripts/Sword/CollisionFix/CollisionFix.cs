using MarkusSecundus.PhysicsSwordfight.PhysicsUtils;
using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MarkusSecundus.PhysicsSwordfight.Sword.Collisions
{
    /// <summary>
    /// Component that solves the issue of sword blades tunneling through one-another.
    /// 
    /// <para>
    /// CCD doesn't work in this case because according to Unity docs <see href="https://docs.unity3d.com/Manual/ContinuousCollisionDetection.html"/>, classical dynamic collision detection totally ignores angular motion. 
    /// Speculative CCD seems to suffer from tunneling just as much as DCD does.
    /// </para>
    /// <para>
    /// The way this component solves the issue is by adding additional big collider for every other sword, that collides just with that one other sword and is rotated to always face the other sword's blade.
    /// For details see <see cref="ContrarianColliderBase"/>.
    /// </para>
    /// </summary>
    [Tooltip("Component that solves the issue of sword blades tunneling through one-another")]
    [RequireComponent(typeof(SwordDescriptor)), RequireComponent(typeof(Rigidbody))]
    public partial class CollisionFix : MonoBehaviour 
    {
        /// <summary>
        /// <see cref="ColliderLayer"/> in which all collision-fix colliders should operate.
        /// </summary>
        [Tooltip("Layer in which all collision-fix colliders should operate")]
        public static int TargetLayer => ColliderLayer.CollisionFix;

        /// <summary>
        /// Config to be used by all created <see cref="Fixer"/> instances
        /// </summary>
        [SerializeField]
        [Tooltip("Config to be used by all created Fixer instances")]
        ContrarianCollider.Configuration ColliderConfig = ContrarianCollider.Configuration.Default;

        [Tooltip("Distance from the sword in which fixer colliders would get created")]
        [SerializeField] private float ActivationRadius = 3f;

        /// <summary>
        /// Hierarchies whose member-objects should be ignored when creating fixer colliders
        /// </summary>
        [Tooltip("Hierarchies whose member-objects should be ignored when creating fixer colliders")]
        [SerializeField] Transform[] HierarchiesToIgnore;

        /// <summary>
        /// All colliders of this rigidbody - for disabling their collisions with other <see cref="Fixer"/>s
        /// </summary>
        public IReadOnlyList<Collider> AllColliders { get; private set; }

        new Rigidbody rigidbody { get; set; }
        SwordDescriptor swordDescriptor { get; set; }

        CollisionFixManager manager;

        CallbackedTrigger instanceCreator;
        void Awake()
        {
            this.rigidbody = GetComponent<Rigidbody>();
            this.swordDescriptor = GetComponent<SwordDescriptor>();
            this.AllColliders = GetComponentsInChildren<Collider>();

            instanceCreator = transform.CreateChild("trigger").AddComponent<CallbackedTrigger>()
                .Add<SphereCollider>(c => c.radius = ActivationRadius)
                .Init(CollisionFix.TargetLayer, onEnter: AreaEntered);

            manager = GameObjectHelpers.GetUtilComponent<CollisionFixManager>();
            manager.Register(this);
        }

        void OnDestroy()
        {
            manager?.Unregister(this);
        }

        bool IsInIgnoreList(Transform t) => t == this.transform || HierarchiesToIgnore.Any(h => t.IsDescendantOf(h));

        void AreaEntered(Collider collider)
        {
            if (manager.TryFindFixer(collider, out var other) && !IsInIgnoreList(other.transform))
            {
                manager.EnableFixer(this, other);
                foreach (var c in instanceCreator.Colliders) other.SetIgnoreCollisions(c);
            }
            else
            {
                foreach (var c in instanceCreator.Colliders) Physics.IgnoreCollision(c, collider);
            }
        }

        void SetIgnoreCollisions(Collider collider, bool ignoreCollisions = true)
        {
            foreach (var c in AllColliders) Physics.IgnoreCollision(c, collider, ignoreCollisions);
        }

        /// <summary>
        /// Component responsible for positioning a fixer-collider to prevent tunneling of a specific target sword.
        /// </summary>
        public class Fixer : ContrarianColliderBase
        {
            /// <summary>
            /// Sword where this fixer collider is placed
            /// </summary>
            [Tooltip("Sword where this fixer collider is placed")]
            public CollisionFix Host;
            /// <summary>
            /// The other sword whose tunneling should be prevented
            /// </summary>
            [Tooltip("The other sword whose tunneling should be prevented")]
            public CollisionFix Target;

            /// <inheritdoc/>
            protected override ScaledRay GetHost() => Host.swordDescriptor.SwordAsRay();
            /// <inheritdoc/>
            protected override ScaledRay GetTarget() => Target.swordDescriptor.SwordAsRay();

            /// <summary>
            /// Set all colliders of this fixer to not collide with specified sword
            /// </summary>
            /// <param name="fix"></param>
            public void SetIgnoreCollisions(CollisionFix fix) => fix.SetIgnoreCollisions(this.collider);

            SphereCollider trigger;
            /// <summary>
            /// Fluently initialize this component from a script
            /// </summary>
            /// <param name="host">Sword where this fixer collider is to be placed</param>
            /// <param name="target">The other sword whose tunneling should be prevented</param>
            /// <returns><c>this</c> for chaining purposes</returns>
            public Fixer Init(CollisionFix host, CollisionFix target)
            {
                (this.Host, this.Target) = (host, target);
                this.Config = host.ColliderConfig;
                this.SetUp(host.rigidbody);
                this.SetIgnoreCollisions(host);
                trigger = gameObject.AddComponent<SphereCollider>();
                trigger.isTrigger = true;
                trigger.radius = host.ActivationRadius;
                return this;
            }

            void OnTriggerEnter(Collider other)
            {
                if (Host.manager.TryFindFixer(other, out var justHit))
                {
                    if (justHit != Target)
                        justHit.SetIgnoreCollisions(this.collider);
                    justHit.SetIgnoreCollisions(trigger);
                }
                else
                {
                    Physics.IgnoreCollision(trigger, other);
                }
            }
        }

    }
}