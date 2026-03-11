using Unity.Cinemachine;
using UnityEngine;

public enum CameraState
{
    Free, // "Normal" mode, camera follows the player's directional movement
    LockOn, // Camera is locked on to a target and rotates to face it
    Static, // Camera is in a fixed point in a room and doesn't move
    FPS // Triggered by the player to freely look around
}

public class CameraController : MonoBehaviour
{
    public static CameraController Instance {get; private set;}
    [Header("References")]
    [SerializeField] private CinemachineCamera cam;
    [SerializeField] private PlayerController player;
    [SerializeField] private CameraShake shake;

    [Header("Free Look Settings")]
    [SerializeField, Tooltip("The angle in degrees within which the camera will not move to align with the player's forward direction.")]
    private float forwardDeadzone = 25f;
    [SerializeField, Tooltip("The angle in degrees within which the camera will not move to align with the player's back direction.")]
    private float backDeadzone = 155f;
    [SerializeField, Tooltip("The maximum speed at which the camera can rotate in angle per second.")]
    private float maxCameraSpeed = 120f;

    [Header("Auto Align Settings")]
    [SerializeField, Tooltip("Time in seconds to wait before aligning the camera to the player's forward direction after the player stops moving.")]
    private float timeBeforeAlign = 5.0f;
    [SerializeField, Tooltip("The speed at which the camera will rotate to align, in degrees per second. Slower than free.")]
    private float assistCameraSpeed = 60f;

    private CameraState state = CameraState.Free;
    CinemachineOrbitalFollow orbital;
    private float alignTimer = 0f;
    private bool prevStrafe;
    private float lockedStrafeYaw;

    public void SetState(CameraState newState)
    {
        state = newState;
    }

    void Awake()
    {
        Instance = this;
        if (!cam) cam = GetComponent<CinemachineCamera>();
        orbital = cam != null ? cam.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineOrbitalFollow : null; 
    }

    // Handle state changing here and actual
    // camera movement in lateupdate
    void Update()
    {
        // Revert to free if our lock target disappears.
        if(state == CameraState.LockOn && player.stateData.lockOnTarget == null)
        {
            state = CameraState.Free;
        }
    }

    void LateUpdate()
    {
        switch(state)
        {
            case CameraState.Free:
                FreeLook();
                break;
            case CameraState.LockOn:
                LockOn();
                break;
            case CameraState.Static:
                break;
            case CameraState.FPS:
                break;
        }
    }

    /// <summary>
    /// Base camera movement of the game.
    /// For exploring and adventuring.
    /// 
    /// Movement is based on player input, and will
    /// try to ease itself towards the direction the player is moving in.
    /// </summary>
    void FreeLook()
    {
        bool strafing = player.stateData.isStrafing;

        // Lock camera yaw when strafe begins
        if (strafing && !prevStrafe)
        {
            Vector3 playerForward = player.transform.forward;
            playerForward.y = 0f;
            playerForward.Normalize();
            lockedStrafeYaw = Quaternion.LookRotation(playerForward).eulerAngles.y;
        }
        prevStrafe = strafing;

        // If strafing without a target, hold camera direction
        if (strafing && player.stateData.lockOnTarget == null)
        {
            orbital.HorizontalAxis.Value = Mathf.LerpAngle(
                orbital.HorizontalAxis.Value,
                lockedStrafeYaw,
                assistCameraSpeed * Time.deltaTime
            );
            return;
        }
        // Decided by player input atm
        bool isMoving = player.stateData.moveInput.sqrMagnitude > 0.01f;

        // Camera dir
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        // The direction we want the camera to be facing
        Vector3 intendedDirection = player.stateData.moveDirection;
        intendedDirection.y = 0f;

        if (intendedDirection.sqrMagnitude > 0.0001f)
            intendedDirection.Normalize();

        // The speed at which the camera rotates to align with intended dir
        float cameraSpeed = maxCameraSpeed;

        if (!isMoving)
        {
            alignTimer += Time.deltaTime;
            if (alignTimer >= timeBeforeAlign)
            {
                intendedDirection = player.transform.forward;
                intendedDirection.y = 0f;
                intendedDirection.Normalize();
                cameraSpeed = assistCameraSpeed;
            }
        }
        else
        {
            alignTimer = 0f;
        }

        float angle = Vector3.SignedAngle(camForward, intendedDirection, Vector3.up);
        float absAngle = Mathf.Abs(angle);
        float rotationSpeed = 0f;

        if (absAngle > forwardDeadzone && absAngle < backDeadzone)
        {
            float t = Mathf.Sin(angle * Mathf.Deg2Rad);
            rotationSpeed = t * cameraSpeed;
        }

        orbital.HorizontalAxis.Value += rotationSpeed * Time.deltaTime;
    }

    void LockOn()
    {
         if (!player || !player.stateData.lockOnTarget) return;

        // direction from player to target
        Vector3 toTarget = player.stateData.lockOnTarget.position - player.transform.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude < 0.001f)
            return;

        float targetYaw = Quaternion.LookRotation(toTarget).eulerAngles.y;

        // set orbital camera yaw
        orbital.HorizontalAxis.Value = targetYaw;
    }

    public void CameraShake(float intensity, float time)
    {
        if (shake) shake.ShakeCamera(intensity, time);
    }
}
