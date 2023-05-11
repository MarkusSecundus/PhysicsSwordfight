using DG.Tweening;
using MarkusSecundus.PhysicsSwordfight.PhysicsUtils;
using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using MarkusSecundus.PhysicsSwordfight.Utils.Geometry;
using MarkusSecundus.PhysicsSwordfight.Utils.Graphics;
using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Sword.Collisions
{
    /// <summary>
    /// Simple implementation of <see cref="ContrarianColliderBase"/> to enable testing if tunneling-prevention works in controlled environment without all the machinery of <see cref="CollisionFix"/>.
    /// </summary>
    public class ContrarianCollider : ContrarianColliderBase
    {
        /// <summary>
        /// Sword where this fixer collider is placed
        /// </summary>
        [Tooltip("Sword where this fixer collider is placed")]
        public SwordDescriptor Target;
        /// <summary>
        /// The other sword whose tunneling should be prevented
        /// </summary>
        [Tooltip("The other sword whose tunneling should be prevented")]
        public SwordDescriptor Host;
        /// <inheritdoc/>
        protected override ScaledRay GetTarget() => Target.SwordAsRay();
        /// <inheritdoc/>
        protected override ScaledRay GetHost() => Host.SwordAsRay();

        /// <inheritdoc/>
        protected override void Awake()
        {
            base.Awake();
            SetUp(Host.GetComponent<Rigidbody>());
            DisableCollisionsWithParent();
        }
        void DisableCollisionsWithParent()
        {
            foreach (var c in Host.gameObject.GetComponentsInChildren<Collider>())
                this.IgnoreCollisions(c, true);
        }
    }

    /// <summary>
    /// Component for preventing tunneling of two sword-like objects by creating a big collider that lives inside one of them and rotates to always face the other.
    /// </summary>
    public abstract class ContrarianColliderBase : MonoBehaviour
    {
        /// <summary>
        /// Configurable parameters
        /// </summary>
        [System.Serializable]
        public struct Configuration
        {
            /// <summary>
            /// Diameter of the collider.
            /// </summary>
            [Tooltip("Diameter of the collider")]
            public float ColliderDepth;
            /// <summary>
            /// Max distance of the blades at which tunneling is prevented. 
            /// </summary>
            [Tooltip("Max distance of the blades at which tunneling is prevented")]
            public float MaxTunnelingDetectionDistance;
            /// <summary>
            /// Added diameter to the collider that is ignored in it's positioning computation. Used for preventing a sword getting stuck between other sword and its collider.
            /// </summary>
            [Tooltip("Added diameter to the collider that is ignored in it's positioning computation. Used for preventing a sword getting stuck between other sword and its collider")]
            public float Overreach;

            /// <summary>
            /// Default value for the editor
            /// </summary>
            public static Configuration Default => new Configuration { ColliderDepth = 1f, MaxTunnelingDetectionDistance = 0.3f, Overreach = 0.1f };
        }

        /// <summary>
        /// Configurable parameters
        /// </summary>
        public Configuration Config = Configuration.Default;

        /// <summary>
        /// Ray describing the sword where this fixer collider is placed
        /// </summary>
        protected abstract ScaledRay GetTarget();
        /// <summary>
        /// Ray describing the other sword whose tunneling should be prevented
        /// </summary>
        protected abstract ScaledRay GetHost();

        /// <summary>
        /// Collider that performs the tunneling-prevention job
        /// </summary>
        protected new BoxCollider collider;


        Rigidbody HostRigidbody;

        /// <summary>
        /// Makes sure this collision fixing utility doesn't collide with specified non-target collider
        /// </summary>
        /// <param name="other">Collider to be ignored</param>
        /// <param name="shouldIgnore"><c>true</c> if the collisions should be ignored, <c>false</c> if they should not</param>
        public void IgnoreCollisions(Collider other, bool shouldIgnore = true) => Physics.IgnoreCollision(collider, other, shouldIgnore);

        /// <summary>
        /// Standard Unity message for component initialization
        /// </summary>
        protected virtual void Awake(){}

        /// <summary>
        /// Initialize the collider from script
        /// </summary>
        /// <param name="hostRigidbody">The sword where this fixer collider is to be placed</param>
        protected void SetUp(Rigidbody hostRigidbody)
        {
            HostRigidbody = hostRigidbody.GetComponent<Rigidbody>();
            var bladeLength = GetHost().length;
            var sideLength = (Config.ColliderDepth + Config.Overreach) / Mathf.Sqrt(2f);

            collider = gameObject.AddComponent<BoxCollider>();
            collider.size = new Vector3(sideLength, bladeLength, sideLength);
            collider.gameObject.layer = ColliderLayer.CollisionFix;

            gameObject.transform.SetParent(HostRigidbody.transform);
        }


        /// <summary>
        /// Calls <see cref="UpdateColliderPosition"/>
        /// </summary>
        protected virtual void FixedUpdate()
        {
            UpdateColliderPosition();
        }


        Vector3 lastDirection = NumericConstants.NaNVector3;
        /// <summary>
        /// Updates position of the collider to face the target sword
        /// </summary>
        protected void UpdateColliderPosition()
        {
            ScaledRay thisSword = GetHost(), otherSword = GetTarget();
            var directionRay = otherSword.GetShortestScaledRayConnection(thisSword);
            var opposite = directionRay.origin;


#if true
            ScaledRay thisSwordPrediction = AccountForCurrentMotion(thisSword, HostRigidbody, Time.fixedDeltaTime);
            Debug.DrawLine(thisSwordPrediction.origin, thisSwordPrediction.end, Color.yellow);
            thisSword = thisSwordPrediction;
#endif
#if false
        DrawHelpers.DrawWireSphere(opposite, 0.1f, (a, b) => Debug.DrawLine(a, b, Color.red));
        DrawHelpers.DrawWireSphere(directionRay.end, 0.05f, (a, b) => Debug.DrawLine(a, b, Color.yellow));
        Debug.DrawLine(directionRay.origin, directionRay.end, Color.green);
#endif


            var bladeCenter = thisSword.origin + thisSword.direction * 0.5f;
            var bladeDirection = thisSword.direction.normalized;
            var planeToContainCollider = new Plane(bladeDirection, bladeCenter);

            var direction = (bladeCenter - planeToContainCollider.ClosestPointOnPlane(opposite)).normalized;
            if (direction == Vector3.zero) return;
            if (directionRay.length < Config.MaxTunnelingDetectionDistance && direction.Dot(lastDirection) < 0)
            {
                direction = -direction;
            }
            lastDirection = direction;
            var position = bladeCenter + direction * Config.ColliderDepth * 0.5f;


            collider.transform.position = position;
            collider.transform.rotation = Quaternion.AngleAxis(45f, bladeDirection) * Quaternion.LookRotation(direction, bladeDirection);

#if false
            debugCircle(directionRay.origin, Color.red);
            debugCircle(directionRay.end, Color.blue);
            Debug.DrawLine(directionRay.origin, directionRay.end, Color.magenta);

            void debugCircle(Vector3 v, Color c) => DrawHelpers.DrawWireSphere(v, 0.02f, (a, b) => Debug.DrawLine(a, b, c), 5, 40);
#endif
        }

        static ScaledRay AccountForCurrentMotion(ScaledRay position, Rigidbody rb, float delta)
        {
            var toRotate = Quaternion.Euler((rb.angularVelocity + 0f * rb.GetAccumulatedTorque() * delta / rb.mass) * delta);
            var toMove = (rb.velocity + 0f * rb.GetAccumulatedForce() * delta / rb.mass) * delta;

            var (origin, end) = (position.origin, position.end);

            (origin, end) = (rotate(origin), rotate(end));
            (origin, end) = (origin + toMove, end + toMove);

            return ScaledRay.FromPoints(origin, end);

            Vector3 rotate(Vector3 v) => rb.transform.LocalToGlobal(toRotate * rb.transform.GlobalToLocal(v));
        }

#if false
        private void OnDrawGizmos()
        {
            var thisSword = GetHost();
            var otherSword = GetTarget();
            var toDraw = thisSword.GetShortestRayConnection(otherSword);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(toDraw.origin, toDraw.direction);
        }
#endif
    }
}