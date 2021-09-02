using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class SimpleMovementController : MonoBehaviour
{
    public CharacterController character;
    public XRRig rig;
    public float accelerationSmoothing = 1f;
    public XRNode leftInputSource;
    public XRNode rightInputSource;
    private Vector2 inputAxis;
    public float additionalCharacterHeight = 0.2f;

    private Vector3 flyingVelocityVector = new Vector3(0,0,0);
    private Vector3 fallingVelocityVector = new Vector3(0,0,0);

    // Flying.
    public float maxFlyingSpeed = 50f;
    public float flyingAcceleration = 70f;
    private float flyingVelocity = 0f;
    private bool fly;
    private bool isFlying = false;
    private bool flyingToggleCooldown = false;

    // Walking.
    public float maxWalkingSpeed = 5;
    public float gravity = -19.81f;
    private float fallingVelocity;

    // Flashlight.
    public Light flashlight;
    private bool flashlightToggleCooldown = false;

    // Update is called once per frame
    void Update()
    {
        // Left hand for movement
        InputDevice leftDevice = InputDevices.GetDeviceAtXRNode(leftInputSource);
        leftDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);

        // Right hand for button inputs.
        InputDevice rightDevice = InputDevices.GetDeviceAtXRNode(rightInputSource);
        
        bool toggleFlying = false;
        rightDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out toggleFlying);
        if(toggleFlying && !flyingToggleCooldown) {
            isFlying = !isFlying;
            Invoke("ResetFlyingCooldown", 1.0f);
            flyingToggleCooldown = true;
        }

        bool toggleFlashlight = false;
        rightDevice.TryGetFeatureValue(CommonUsages.triggerButton, out toggleFlashlight);
        if(toggleFlashlight && !flashlightToggleCooldown) {
            flashlight.gameObject.SetActive(!flashlight.gameObject.activeSelf);
            Invoke("ResetFlashlightCooldown", 0.5f);
            flashlightToggleCooldown = true;
        }
    }

    void ResetFlyingCooldown() {
        flyingToggleCooldown = false;
    }

    void ResetFlashlightCooldown() {
        flashlightToggleCooldown = false;
    }

    private void FixedUpdate() {
        CapsuleFollowHeadset();

        // Calculate flying velocity vector.
        if(flyingVelocity < 0.1f) {
            flyingVelocity = 0;
        }

        if(inputAxis.y > -0.05f && inputAxis.y < 0.05f && flyingVelocity != 0) {
            flyingVelocity -= flyingAcceleration * Time.fixedDeltaTime;
        } else if((inputAxis.y < -0.05f || inputAxis.y > 0.05f) && isFlying) {
            // TODO: NEED TO HANDLE BACKWARDS ACCELERATION.
            flyingVelocity += flyingAcceleration * inputAxis.y * Time.fixedDeltaTime;
            
            if(flyingVelocity > maxFlyingSpeed) {
                flyingVelocity = maxFlyingSpeed;
            }
        }

        Vector3 headAngles = rig.cameraGameObject.transform.eulerAngles;
        Quaternion headRotation = Quaternion.Euler(headAngles.x, headAngles.y, headAngles.z);
        Vector3 flyDirection = headRotation * Vector3.forward;

        flyingVelocityVector = flyDirection * flyingVelocity;

        // Calculate falling velocity vector.
        if(isFlying && fallingVelocity >= 0) {
            fallingVelocity = 0;
        }

        bool grounded = isGrounded();
        if (grounded && !isFlying) {
            fallingVelocity = 0;
        } else if(!isFlying) {
            fallingVelocity += gravity * Time.fixedDeltaTime;
        } else if(isFlying && fallingVelocity != 0) {
            fallingVelocity += flyingAcceleration * Time.fixedDeltaTime;
        }

        fallingVelocityVector = Vector3.up * fallingVelocity;

        if(isFlying) {  
            // Flying movement.
            character.Move(flyingVelocityVector * Time.fixedDeltaTime);

            // Account for any previous falling velocity.
            character.Move(fallingVelocityVector * Time.fixedDeltaTime);
            
        } else {
            // Walking movement.
            Quaternion headYaw = Quaternion.Euler(0, rig.cameraGameObject.transform.eulerAngles.y, 0);
            Vector3 direction = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y);
            character.Move(direction * Time.fixedDeltaTime * maxWalkingSpeed);
            
            // Account for falling.
            character.Move(fallingVelocityVector * Time.fixedDeltaTime);

            // Account for any previous flying velocity.
            character.Move(flyingVelocityVector * Time.fixedDeltaTime);
        }
    }

    void CapsuleFollowHeadset() {
        character.height = rig.cameraInRigSpaceHeight + additionalCharacterHeight;
        Vector3 capsuleCenter = transform.InverseTransformPoint(rig.cameraGameObject.transform.position);
        character.center = new Vector3(capsuleCenter.x, character.height/2 + character.skinWidth, capsuleCenter.z);
    }

    bool isGrounded() {
        Vector3 rayStart = transform.TransformPoint(character.center);
        float rayLength = character.center.y + 0.01f;
        bool rayHasHit = Physics.SphereCast(rayStart, character.radius - 0.2f, Vector3.down, out RaycastHit hitInfo, rayLength);
        return rayHasHit;
    }

}
