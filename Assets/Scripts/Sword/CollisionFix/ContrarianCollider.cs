using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ContrarianCollider : ContrarianColliderBase
{
    public SwordDescriptor Target, Host;
    protected override ScaledRay GetTarget() => Target.SwordBladeAsRay();
    protected override ScaledRay GetHost() => Host.SwordBladeAsRay();

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

public abstract class ContrarianColliderBase : MonoBehaviour
{
    [System.Serializable]
    public struct Configuration
    {
        public Mode mode;
        public float ColliderDepth, Tolerance, Overreach, ForceMultiplierForStandaloneMode;

        public static Configuration Default => new Configuration { mode = Mode.Hosted, ColliderDepth = 1f, Tolerance = 0.3f, Overreach = 0.1f, ForceMultiplierForStandaloneMode = -1f };
    }

    public Configuration Config = Configuration.Default;

    public enum Mode
    {
        Standalone, Hosted
    }

    protected abstract ScaledRay GetTarget();
    protected abstract ScaledRay GetHost();


    protected new BoxCollider collider;


    private Rigidbody HostRigidbody;

    public void IgnoreCollisions(Collider other, bool shouldIgnore = true) => Physics.IgnoreCollision(collider, other, shouldIgnore);


    protected virtual void Awake()
    {
        gameObject.layer = ColliderLayers.CollisionFix; 
        collider = gameObject.AddComponent<BoxCollider>();

        StartCoroutine(LateFixedUpdateCaller());
        IEnumerator LateFixedUpdateCaller()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                LateFixedUpdate();
            }
        }
    }


    protected void SetUp(Rigidbody hostRigidbody)
    {
        HostRigidbody = hostRigidbody.GetComponent<Rigidbody>();
        var bladeLength = GetHost().length;
        var sideLength = (Config.ColliderDepth + Config.Overreach) / Mathf.Sqrt(2f);

        collider.size = new Vector3(sideLength, bladeLength, sideLength);

        if (Config.mode == Mode.Standalone)
        {
            var rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }else if(Config.mode == Mode.Hosted)
        {
            gameObject.transform.SetParent(HostRigidbody.transform);
        }
    }



    private void OnCollisionEnter(Collision collision) => OnCollision(collision);
    private void OnCollisionStay(Collision collision) => OnCollision(collision);
    private void OnCollisionExit(Collision collision) => OnCollision(collision);

    void OnCollision(Collision collision)
    {
        if (Config.mode == Mode.Standalone)
        {
            foreach (var c in collision.IterateContacts())
            {
                HostRigidbody.AddForceAtPosition(c.impulse* Config.ForceMultiplierForStandaloneMode, c.point, ForceMode.VelocityChange);
                //if (c.impulse == Vector3.zero) continue;
                //Debug.Log($"fr. {Time.frameCount} - adding force {c.impulse.ToStringPrecise()}");
                //DrawHelpers.DrawWireSphere(c.point, 0.03f, (a, b) => Debug.DrawLine(a, b, Color.green));
                //Debug.DrawLine(c.point, c.point + c.impulse, Color.red);
                //Debug.DrawLine(c.point, c.point + c.normal, Color.blue);
            }
        }
    }

    protected virtual void LateFixedUpdate()
    {
        //Debug.Log($"fr.{Time.frameCount} Late Update called!");
        //UpdateColliderPosition();
    }
    protected virtual void FixedUpdate()
    {
        UpdateColliderPosition();
    }

    private Vector3 lastDirection = VectorUtils.NaNVector3;
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

        var fixer = collider.gameObject;
        var bladeCenter = thisSword.origin + thisSword.direction * 0.5f;
        var bladeDirection = thisSword.direction.normalized;
        var planeToContainCollider = new Plane(bladeDirection, bladeCenter);

        var direction = (bladeCenter - planeToContainCollider.ClosestPointOnPlane(opposite)).normalized;
        if (direction.Dot(lastDirection) < 0 && directionRay.length < Config.Tolerance)
        {
            direction = -direction;
        }
        lastDirection = direction;
        var position = bladeCenter + direction * Config.ColliderDepth * 0.5f;

        fixer.transform.position = position;
        fixer.transform.rotation = Quaternion.AngleAxis(45f, bladeDirection) * Quaternion.LookRotation(direction, bladeDirection);
    }

    private static ScaledRay AccountForCurrentMotion(ScaledRay position, Rigidbody rb, float delta)
    {
        var toRotate = Quaternion.Euler(rb.angularVelocity * delta);
        var toMove = rb.velocity * delta;

        var (origin, end) = (position.origin, position.end);

        (origin, end) = (rotate(origin), rotate(end));
        (origin, end) = (origin + toMove, end + toMove);

        return ScaledRay.FromPoints(origin, end);

        Vector3 rotate(Vector3 v) => rb.transform.LocalToGlobal(toRotate * rb.transform.GlobalToLocal(v));
    }

    /*private void OnDrawGizmos()
    {
        Ray thisSword = SwordDescriptor.SwordBladeAsRay(), otherSword = GetTarget().SwordBladeAsRay();
        var toDraw = thisSword.GetShortestRayConnection(otherSword);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(toDraw);
    }*/
}
