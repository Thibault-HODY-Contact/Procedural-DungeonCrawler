using Unity.Cinemachine;
using UnityEngine;

public class GasCameraFeedback : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GasSpread gas;
    [SerializeField] private Transform playerPosition;
    [SerializeField] private CinemachineBasicMultiChannelPerlin cameraNoise;

    [Header("Camera Shake")]
    [SerializeField] private float maxAmplitude = 1.2f;
    [SerializeField] private float frequency = 1.5f;
    [SerializeField] private float transitionSpeed = 2f;

    private float targetAmplitude;

    private void Update()
    {
        if (gas == null || playerPosition == null ||
            cameraNoise == null)
            return;

        Vector3 playerPos = playerPosition.position;
        Vector3 gasPos = gas.transform.position;

        float distance = Vector2.Distance(
            new Vector2(playerPos.x, playerPos.z),
            new Vector2(gasPos.x, gasPos.z)
        );

        bool isInGas = distance <= gas.CurrentRadius;

        targetAmplitude = isInGas ? maxAmplitude : 0f;
    }
}
