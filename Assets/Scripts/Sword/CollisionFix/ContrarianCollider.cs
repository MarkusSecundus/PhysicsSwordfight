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
        SetUpCollider();
        DisableCollisionsWithParent();
    }
    void DisableCollisionsWithParent()
    {
        foreach (var c in HostDescriptor.gameObject.GetComponentsInChildren<Collider>())
            this.IgnoreCollisions(c, true);
    }
}

public abstract class ContrarianColliderBase : MonoBehaviour
{
    private new BoxCollider collider;

    public float ColliderDepth = 1f, Tolerance = 0.3f;
    public SwordDescriptor HostDescriptor;
    protected abstract SwordDescriptor GetTarget();

    private Rigidbody HostRigidbody;


    public void IgnoreCollisions(Collider other, bool shouldIgnore = true) => Physics.IgnoreCollision(collider, other, shouldIgnore);


    protected virtual void Start()
    {
        var rb = gameObject.AddComponent<Rigidbody>();rb.isKinematic = true;
        //gameObject.transform.SetParent(SwordDescriptor.transform);
        gameObject.layer = ColliderLayers.CollisionFix;
        collider = gameObject.AddComponent<BoxCollider>();
    }


    protected void SetUpCollider()
    {

        var bladeLength = HostDescriptor.SwordTip.position.Distance(HostDescriptor.SwordAnchor.position);
        var sideLength = ColliderDepth / Mathf.Sqrt(2f);

        collider.size = new Vector3(sideLength, bladeLength, sideLength);
        //var triggerCol = gameObject.AddComponent<BoxCollider>();triggerCol.isTrigger = true;triggerCol.size = collider.size * 2;
        HostRigidbody = HostDescriptor.GetComponent<Rigidbody>();

        //var joint = gameObject.AddComponent<FixedJoint>();
        //joint.connectedBody = HostRigidbody;
    }

    private void OnCollisionEnter(Collision collision) => OnCollision(collision);
    private void OnCollisionStay(Collision collision) => OnCollision(collision);
    private void OnCollisionExit(Collision collision) => OnCollision(collision);


    void OnCollision(Collision collision)
    {
#if true
        foreach (var c in collision.IterateContacts())
        {
            HostRigidbody.AddForceAtPosition(-c.impulse, c.point, ForceMode.Acceleration);
            if (c.impulse == Vector3.zero) continue;
            Debug.Log($"fr. {Time.frameCount} - adding force {c.impulse.ToStringPrecise()}");
        }
#endif
    }


    protected virtual void FixedUpdate()
    {
        UpdateColliderPosition();
    }

    private Vector3 lastDirection = VectorUtils.NaNVector3;
    protected void UpdateColliderPosition()
    {
        ScaledRay thisSword = HostDescriptor.SwordBladeAsRay(), otherSword = GetTarget().SwordBladeAsRay();
        var directionRay = otherSword.GetShortestScaledRayConnection(thisSword);
        var opposite = directionRay.origin;

#if false
        DrawHelpers.DrawWireSphere(opposite, 0.1f, (a, b) => Debug.DrawLine(a, b, Color.red));
        DrawHelpers.DrawWireSphere(directionRay.end, 0.05f, (a, b) => Debug.DrawLine(a, b, Color.yellow));
        Debug.DrawLine(directionRay.origin, directionRay.end, Color.green);
#endif

        var fixer = collider.gameObject;
        Vector3 bladeAnchor = HostDescriptor.SwordAnchor.position, bladeTip = HostDescriptor.SwordTip.position;
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

        fixer.transform.position = position;
        fixer.transform.rotation = Quaternion.AngleAxis(45f, bladeDirection) * Quaternion.LookRotation(direction, bladeDirection);
    }

    /*private void OnDrawGizmos()
    {
        Ray thisSword = SwordDescriptor.SwordBladeAsRay(), otherSword = GetTarget().SwordBladeAsRay();
        var toDraw = thisSword.GetShortestRayConnection(otherSword);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(toDraw);
    }*/
}
