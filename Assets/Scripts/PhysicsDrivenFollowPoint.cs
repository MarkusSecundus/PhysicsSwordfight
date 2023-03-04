using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class PhysicsDrivenFollowPoint : PhysicsDrivenFollowPointBase
{
    [SerializeField] private Transform ToFollow;
    protected override Vector3 GetFollowPosition() => ToFollow.position;
}



[RequireComponent(typeof(Rigidbody))]
public abstract class PhysicsDrivenFollowPointBase : MonoBehaviour
{
    protected abstract Vector3 GetFollowPosition();

    [System.Serializable]
    public class Configuration
    {
        public float DriveForce = 1f, MaxVelocity = 1f, SlowdownDistance = 0.1f, SlowdownRate = 2f, TestParam=0.05f;
    }

    public Configuration Config = new Configuration();


    protected Rigidbody rb;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        DoStep(Time.fixedDeltaTime);
    }

    Vector3 lastAppliedForce = Vector3.zero;
    void DoStep(float delta)
    {
        var c = Config;
        var toFollow = GetFollowPosition();
        var toMove = toFollow - rb.position;
        var direction = toMove.Normalized(out var distance);
        //Debug.Log($"rb{rb.position}->{toFollow}  [{distance}*{direction}]");
        var velocityDirection = (direction - rb.velocity.normalized).normalized;

        var maxVelocityMagnitude = ComputeMaxVelocityMagnitude();

        var forceMagnitudeToApply = c.DriveForce * delta;   //multiply by delta to get the immediate velocity change

        var forceToApply = velocityDirection * forceMagnitudeToApply;
        var resultVelocity = rb.velocity + forceToApply;


        if (resultVelocity.magnitude > maxVelocityMagnitude)
        {
            var clampedTargetVelocity = velocityDirection * maxVelocityMagnitude;
            forceToApply = (clampedTargetVelocity - rb.velocity);

            
            rb.AddForce(forceToApply, ForceMode.VelocityChange);
            lastAppliedForce = forceToApply;
            //Debug.Log($"Clamped.. add{clampedTargetVelocity.ToStringPrecise()} -> vel{rb.velocity.ToStringPrecise()}");
            
        }
        else
        {
            rb.AddForce(forceToApply, ForceMode.VelocityChange);
            //Debug.Log($"Applied.. frc{forceToApply.ToStringPrecise()} -> vel{rb.velocity.ToStringPrecise()}");
        }


        float ComputeMaxVelocityMagnitude()
        {
            var maxVelocity = c.MaxVelocity;
            if (distance < c.SlowdownDistance)
            {
                var interpolationParameter = distance / c.SlowdownDistance; //number between 0 and 1
                var multiplier = Mathf.Pow(interpolationParameter, c.SlowdownRate);
                maxVelocity *= multiplier;
            }
            return maxVelocity;
        }
    }

}



