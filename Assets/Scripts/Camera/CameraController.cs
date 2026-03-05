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

    void Awake()
    {
        Instance = this;
        if (!cam) cam = GetComponent<CinemachineCamera>();
        orbital = cam != null ? cam.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineOrbitalFollow : null; 
    }

    void LateUpdate()
    {
        switch(state)
        {
            case CameraState.Free:
                FreeLook();
                break;
            case CameraState.LockOn:
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
        // Decided by player input atm
        bool isMoving = player.stateData.moveInput.sqrMagnitude > 0.01f;

        // Camera dir
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        // The direction we want the camera to be facing
        Vector3 intendedDirection = player.stateData.moveDirection;
        intendedDirection.y = 0f;
        intendedDirection.Normalize();

        // The speed at which the camera rotates to align with intended dir
        float cameraSpeed = maxCameraSpeed;

        if (!isMoving)
        {
            alignTimer += Time.deltaTime;
            if (alignTimer >= timeBeforeAlign)
            {
                // Update these in the case we need to align to the player
                // while they are not moving
                intendedDirection = player.transform.forward;
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

    public void CameraShake(float intensity, float time)
    {
        if (shake) shake.ShakeCamera(intensity, time);
    }
}
