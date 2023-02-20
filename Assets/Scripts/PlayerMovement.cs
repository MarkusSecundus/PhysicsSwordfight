using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [System.Serializable]
    public class InputMapping
    {
        public KeyCode Jump = KeyCode.LeftShift;
        public InputAxis WalkForwardBackward = InputAxis.Vertical, StrafeLeftRight = InputAxis.Horizontal, LookUpDown = InputAxis.MouseY, LookLeftRight = InputAxis.MouseX, RotateLeftRight = InputAxis.HorizontalSecondary;
    }
    [System.Serializable]
    public class InputMultipliers
    {
        public float WalkForwardBackward = 1f, StrafeLeftRight = 1f, RotateLeftRight = 1f;
        public float LookUpDown = 1f, LookLeftRight = 0.1f;
        public Vector3 Jump = Vector3.up;
        public ForceMode JumpMode = ForceMode.VelocityChange;
        public Vector3Interval LookConstraints; 
        public float InputStabilizationRate = 0.8f;
    }


    public ISwordInput Input;
    public Transform CameraToUse;

    private new Rigidbody rigidbody;

    public InputMapping Mapping = new InputMapping();
    public InputMultipliers Tweaks = new InputMultipliers();


    private Vector3 WalkForwardBackwardBase => transform.forward;
    private Vector3 StrafeLeftRightBase => transform.right;
    private Vector3 RotateLeftRightBase => transform.up;

    private Vector3 LookUpDownBase => default;
    private Vector3 LookLeftRightBase => default;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationY;
    }


    void FixedUpdate()
    {
        DoMoveStep(Time.fixedDeltaTime);
    }

    void DoMoveStep(float delta)
    {
        HandleWalking(delta);
        HandleRotating(delta);
        HandleLooking(delta);
        HandleJumping(delta);
    }




    void HandleWalking(float delta)
    {
        var walkForward = Input.GetAxis(Mapping.WalkForwardBackward) * Tweaks.WalkForwardBackward;
        var strafeLeftRight = Input.GetAxis(Mapping.StrafeLeftRight) * Tweaks.StrafeLeftRight;

        var toMove = (walkForward*WalkForwardBackwardBase + strafeLeftRight*StrafeLeftRightBase); //*delta !!velocity should not be multiplied by delta (opposed to applied force)!

        rigidbody.velocity = toMove.With(y: toMove.y + rigidbody.velocity.y);

        //rigidbody.AddRelativeForce(walkForward*delta);
        //rigidbody.AddRelativeForce(strafeLeftRight*delta);
    }

    void HandleRotating(float delta)
    {
        var axisValue = Input.GetAxis(Mapping.RotateLeftRight);
        var rotateLeftRight = axisValue * Tweaks.RotateLeftRight * RotateLeftRightBase;

        rigidbody.MoveRotation(rigidbody.rotation.WithEuler(x: 0f, z: 0f)); //?! TODO: figure out if and why this is necessary! (especially since those rotation axes are already locked)
        rigidbody.angularVelocity = axisValue==0 /*solid 0 only when the input is non-active*/ ? rigidbody.angularVelocity * Tweaks.InputStabilizationRate : rotateLeftRight; //again, velocity should not be multiplied by delta!
    }

    void HandleLooking(float delta)
    {
        float lookLeftRight = Input.GetAxis(Mapping.LookLeftRight) * Tweaks.LookLeftRight * delta; //here delta makes sense since we are manually moving the camera rotation by some ammount
        float lookUpDown = Input.GetAxis(Mapping.LookUpDown) * Tweaks.LookUpDown * delta;


        var currentCameraRotation = CameraToUse.localRotation.eulerAngles;
        var xRotation = currentCameraRotation.x - lookUpDown;   //for some reason needs to use substraction to not be inverted
        var yRotation = currentCameraRotation.y + lookLeftRight; //no need to clamp this to the reasonable <0;180°> interval - that is taken care of just by forcing the z rotation to 0° (if the player managed to force y outside of this range, that changes z to upside-down a.k.a 180° - reseting that back to 0° by lucky coincidence resets the camera exactly to the reasonable position that one would want in such scenario)
        var rotationToSet = new Vector3(xRotation, yRotation, 0f).ClampEuler(Tweaks.LookConstraints);
        if (CameraToUse != null)
            CameraToUse.localRotation = Quaternion.Euler(rotationToSet);
    }

    void HandleJumping(float delta)
    {
        if (Input.GetKeyDown(Mapping.Jump))
            rigidbody.AddRelativeForce(Tweaks.Jump, Tweaks.JumpMode);
    }



    //transform from local coords to global while preserving the magnitude
    private Vector3 LocalToGlobal(Vector3 local)
    {
        var magnitude = local.magnitude;
        return transform.LocalToGlobal(local).normalized * magnitude;
    }
}

