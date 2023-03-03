using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public abstract class PhysicsDrivenFollowPointBase : MonoBehaviour
{
    protected abstract Vector3 GetFollowPosition();
}



[RequireComponent(typeof(Rigidbody))]
public class PhysicsDrivenFollowPoint : PhysicsDrivenFollowPointBase
{
    [SerializeField] private Transform ToFollow;
    protected override Vector3 GetFollowPosition() => ToFollow.position;

    public float DriveForce = 1f, MaxVelocity = 1f, SlowdownDistance = 0.1f, SlowdownRate = 2f;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        DoStep(Time.fixedDeltaTime);
    }


    void DoStep(float delta)
    {
        var toMove = ToFollow.position - transform.position;
        var direction = toMove.Normalized(out var distance);

        //var velocityDirection = (toMove - rb.velocity).normalized;//(direction - rb.velocity.normalized).normalized;
        var velocityDirection = (direction - rb.velocity.normalized).normalized;

        var maxVelocityMagnitude = ComputeMaxVelocityMagnitude();
        
        var forceMagnitudeToApply = DriveForce * delta;

        var forceToApply = velocityDirection * forceMagnitudeToApply;
        var resultVelocity = rb.velocity + forceToApply;
        if(resultVelocity.magnitude > maxVelocityMagnitude)
        {
            Debug.Log("Reached max velocity");
        }

        //rb.MoveToVelocity(toMove);
        rb.AddForce(forceToApply, ForceMode.VelocityChange);

        Debug.Log($"this{rb.position}, target{ToFollow.position} - dir{direction}, veldir{velocityDirection}, vel{rb.velocity}");

        float ComputeMaxVelocityMagnitude()
        {
            var maxVelocity = MaxVelocity;
            if (distance < SlowdownDistance)
            {
                var interpolationParameter = distance / SlowdownDistance; //number between 0 and 1
                var multiplier = Mathf.Pow(interpolationParameter, SlowdownRate);
                maxVelocity *= multiplier;
            }
            return maxVelocity;
        }
    }

}
