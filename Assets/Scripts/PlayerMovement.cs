using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform mainCamera;

    public float mouseXMultiplier=1f, mouseYMultiplier=1f, keyboardMultiplier=1f;

    public float horizontalRotationMultiplier=1f;

    public ForceMode jumpMode = ForceMode.VelocityChange;

    private CharacterController controller;
    private Rigidbody rb;
    public Transform feetPosition;
    public float feetRadius = 0.1f;
    public LayerMask floorMask;


    public Vector3 jumpForce = Vector3.up;

    public bool MouseOnlyIfNotClicked = true;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
    }


    public bool BindMove = true;

    public float GroundedSpeedThreshold = 0.01f;

    // Update is called once per frame

    private bool IsGrounded => rb.velocity.sqrMagnitude < GroundedSpeedThreshold ||  Physics.CheckSphere(feetPosition.position, feetRadius, floorMask);

    void FixedUpdate()
    {
        var delta = Time.fixedDeltaTime;
        if (Input.GetKeyDown(KeyCode.Tab))
            BindMove = !BindMove;
        
        /*if (!BindMove)
        {
            rb.constraints |= RigidbodyConstraints.FreezeRotationY;
            return;
        }*/

        rb.constraints &= ~RigidbodyConstraints.FreezeRotationY;

        var vertical = Input.GetAxis("Vertical");
        var horizontal = Input.GetAxis("Horizontal");

        float mouseX = default, mouseY = default;
        if(!MouseOnlyIfNotClicked || !Input.GetKey(KeyCode.Mouse0))
        {
            mouseX = Input.GetAxis("HorizontalRotation") * horizontalRotationMultiplier; //Input.GetAxis("Mouse X") * mouseXMultiplier * delta;
            mouseY = Input.GetAxis("Mouse Y") * mouseYMultiplier * delta;
        }

        //Debug.Log($"vertical: {vertical} -- horizontal: {horizontal}\nmX: {mouseX} -- mY: {mouseY}");

        //transform.Rotate(Vector3.up * mouseX);
        rb.MoveRotation(rb.rotation.withEuler(x: 0f, z: 0f));
        rb.angularVelocity = Vector3.up * mouseX;

        if(mainCamera != null && BindMove)
        {
            var currXRotation = mainCamera.localRotation.eulerAngles.x;

            var xRotation = currXRotation - mouseY;//Mathf.Clamp(currXRotation - mouseY, -90f, 90f);
            mainCamera.localRotation = Quaternion.Euler(xRotation, 0, 0);
        }

        var toMove = (transform.forward * vertical + transform.right * horizontal) * delta * keyboardMultiplier;

        if (this.IsGrounded)
        {
            if (controller != null)
                controller.Move(toMove);
            else if (rb != null)
                rb.velocity = new Vector3(toMove.x, rb.velocity.y, toMove.z);

            if (Input.GetKeyDown(KeyCode.Space))
                rb.AddRelativeForce(jumpForce, jumpMode);
        }


#if false

        var mv = new Vector3(horizontal, 0, vertical) * keyboardMultiplier;
        mv = transform.TransformDirection(mv);
        //rb.AddForce(mv, mode);
        oldPosition = transform.position;
        transform.position += mv;
        transform.Rotate(new Vector3(-mouseY*0, mouseX) * mouseMultiplier);
#endif


    }
    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (oldPosition != null) transform.position = oldPosition.Value;
    }

    private void OnCollisionStay(Collision collision)
    {
        OnCollisionEnter(null);   
    }*/


    private void SetRagdoll(bool ragdoll)
    {
        rb.isKinematic = !ragdoll;
    }
}

static class PlayerMovementExtensions
{

    public static Quaternion withEuler(this Quaternion self, float? x = null, float? y = null, float? z = null)
    {
        var eul = self.eulerAngles;
        if (x != null) eul.x = x.Value;
        if (y != null) eul.y = y.Value;
        if (z != null) eul.z = z.Value;
        return Quaternion.Euler(eul);
    }
}

