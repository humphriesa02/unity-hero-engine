using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance {get; private set;}
    [SerializeField] private CinemachineCamera cam;
    [SerializeField] private Transform player;
    [SerializeField] private Transform lockOnTarget;
    [SerializeField] private CameraShake shake;

    private bool fpsMode;
    private bool lockOnMode;

    [SerializeField] private float snapStepDegrees = 22.5f;
    [SerializeField] private float alignSpeed = 10f;

    CinemachineOrbitalFollow orbital;

    void Awake()
    {
        Instance = this;
        if (!cam) cam = GetComponent<CinemachineCamera>();
        orbital = cam != null ? cam.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineOrbitalFollow : null; 
    }

    void LateUpdate()
    {
        if (!orbital || !player) return;

        if (fpsMode) return;

        float desiredYaw = GetDesiredYawDegrees();

        if (snapStepDegrees > 0.001f)
        {
            desiredYaw = Mathf.Round(desiredYaw / snapStepDegrees) * snapStepDegrees;
            desiredYaw = (desiredYaw + 360f) % 360f;
        }
        float current = orbital.HorizontalAxis.Value;
        float next = Mathf.LerpAngle(current, desiredYaw, 1f - Mathf.Exp(-alignSpeed * Time.deltaTime));
        orbital.HorizontalAxis.Value = next;
    }

    private float GetDesiredYawDegrees()
    {
        return player.eulerAngles.y;
    }

    public void CameraShake(float intensity, float time)
    {
        if (shake) shake.ShakeCamera(intensity, time);
    }
}
