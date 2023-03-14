using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ContrarianCollider : ContrarianColliderBase
{
    public SwordDescriptor target;
    protected override SwordDescriptor GetTarget() => target;

    protected override void Start()
    {
        base.Start();
        SetUp();
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
    public enum Mode
    {
        Standalone, Hosted, Jointed
    }

    private new BoxCollider collider;

    public float ColliderDepth = 1f, Tolerance = 0.3f, Overreach = 0.1f;
    public SwordDescriptor Host;
    protected abstract SwordDescriptor GetTarget();

    private Rigidbody HostRigidbody;

    public Mode mode = Mode.Hosted;

    public float ForceMultiplierForStandaloneMode = -1f;

    private ConfigurableJoint joint;


    public void IgnoreCollisions(Collider other, bool shouldIgnore = true) => Physics.IgnoreCollision(collider, other, shouldIgnore);


    protected virtual void Start()
    {
        gameObject.layer = ColliderLayers.CollisionFix; 
        collider = gameObject.AddComponent<BoxCollider>();
    }


    protected void SetUp()
    {
        HostRigidbody = Host.GetComponent<Rigidbody>();
        var bladeLength = Host.SwordBladeAsRay().length;
        var sideLength = (ColliderDepth + Overreach) / Mathf.Sqrt(2f);

        collider.size = new Vector3(sideLength, bladeLength, sideLength);
        {
            var trigger = gameObject.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = collider.size;
        }

        if (mode == Mode.Standalone)
        {
            var rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }else if(mode == Mode.Jointed)
        {
            var hostAsRay = Host.SwordBladeAsRay();
            var hostCenter = (hostAsRay.origin + hostAsRay.end) * 0.5f;
            transform.position = hostCenter + new Vector3(sideLength/2f, 0f, sideLength/2f);
            //transform.localRotation = Quaternion.AngleAxis(45f, Vector3.up);
            var rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 1;
            joint = gameObject.AddComponent<ConfigurableJoint>();
            joint.enablePreprocessing = false;
            joint.connectedBody = HostRigidbody;
            joint.anchor = transform.GlobalToLocal(hostCenter);//new Vector3(-sideLength, 0f, -sideLength);
            joint.axis = new Vector3(0, 1, 0);
            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;
            joint.angularXMotion = ConfigurableJointMotion.Locked;
            joint.angularYMotion = ConfigurableJointMotion.Locked;
            joint.angularZMotion = ConfigurableJointMotion.Locked;
        }
        else if(mode == Mode.Hosted)
        {
            gameObject.transform.SetParent(Host.transform);
        }
    }



    private void OnCollisionEnter(Collision collision) => OnCollision(collision);
    private void OnCollisionStay(Collision collision) => OnCollision(collision);
    private void OnCollisionExit(Collision collision) => OnCollision(collision);

    private void OnTriggerEnter(Collider other) => OnTrigger(other);
    private void OnTriggerStay(Collider other) => OnTrigger(other);
    private void OnTriggerExit(Collider other) => OnTrigger(other);

    void OnCollision(Collision collision)
    {
        if (mode == Mode.Standalone)
        {
            foreach (var c in collision.IterateContacts())
            {
                HostRigidbody.AddForceAtPosition(c.impulse*ForceMultiplierForStandaloneMode, c.point, ForceMode.VelocityChange);
                if (c.impulse == Vector3.zero) continue;
                Debug.Log($"fr. {Time.frameCount} - adding force {c.impulse.ToStringPrecise()}");
                DrawHelpers.DrawWireSphere(c.point, 0.03f, (a, b) => Debug.DrawLine(a, b, Color.green));
                Debug.DrawLine(c.point, c.point + c.impulse, Color.red);
                Debug.DrawLine(c.point, c.point + c.normal, Color.blue);
            }
        }
    }

    void OnTrigger(Collider other)
    {
        var bounds = collider.bounds;
        var cast = Physics.BoxCastAll(bounds.center, bounds.extents, Vector3.zero, collider.transform.rotation);
        foreach(var c in cast)
        {
            DrawHelpers.DrawWireSphere(c.point, 0.03f, (a, b) => Debug.DrawLine(a, b, Color.white));
        }
        Debug.Log($"Triggered! - {cast.Length} contacts!");
    }

    protected virtual void Update()
    {
        UpdateColliderPosition();
    }
    protected virtual void FixedUpdate()
    {
        UpdateColliderPosition();
    }

    private Vector3 lastDirection = VectorUtils.NaNVector3;
    protected void UpdateColliderPosition()
    {
        ScaledRay thisSword = Host.SwordBladeAsRay(), otherSword = GetTarget().SwordBladeAsRay();
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
        Vector3 bladeAnchor = Host.SwordAnchor.position, bladeTip = Host.SwordTip.position;
        var bladeCenter = (bladeAnchor + bladeTip) * 0.5f;
        var bladeDirection = (bladeTip - bladeAnchor).normalized;
        var planeToContainCollider = new Plane(bladeDirection, bladeCenter);

        var direction = (bladeCenter - planeToContainCollider.ClosestPointOnPlane(opposite)).normalized;
        if (direction.Dot(lastDirection) < 0 && directionRay.length < Tolerance)
        {
            direction = -direction;
            Debug.Log($"fr.{Time.frameCount} - Dot was negative!");
        }
        lastDirection = direction;
        var position = bladeCenter + direction * ColliderDepth * 0.5f;

        if(mode == Mode.Standalone || mode == Mode.Hosted)
        {
            fixer.transform.position = position;
            fixer.transform.rotation = Quaternion.AngleAxis(45f, bladeDirection) * Quaternion.LookRotation(direction, bladeDirection);
        }else if(mode == Mode.Jointed)
        {
            //fixer.transform.rotation = Quaternion.LookRotation(direction, bladeDirection);
        }
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
