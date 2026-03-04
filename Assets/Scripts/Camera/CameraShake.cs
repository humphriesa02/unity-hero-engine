using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;
    float shakerTimer;
    float shakerTimerTotal;
    float startingIntensity;

    public void ShakeCamera(float intensity, float time)
    {
        cinemachineBasicMultiChannelPerlin.AmplitudeGain = intensity;

        startingIntensity = intensity;
        shakerTimerTotal = time;
        shakerTimer = time;
    }

    void Update()
    {
        if (shakerTimer > 0)
        {
            shakerTimer -= Time.deltaTime;
            cinemachineBasicMultiChannelPerlin.AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, 1f - shakerTimer / shakerTimerTotal);
        }
    }
}
