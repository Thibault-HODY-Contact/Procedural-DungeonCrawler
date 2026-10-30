using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class FeedbackDamage : MonoBehaviour
{
    [SerializeField] CinemachineBasicMultiChannelPerlin feedback;
    [SerializeField] LifeScript lifeScript;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeAmplitude = 2f;
    [SerializeField] private float shakeFrequency = 2f;

    private Coroutine shakeRoutine;

    void OnEnable()
    {
        if (lifeScript != null) lifeScript.OnDamaged += FeedbackFunc;
    }

    void OnDisable()
    {
        if (lifeScript != null) lifeScript.OnDamaged -= FeedbackFunc;
    }

    void FeedbackFunc()
    {
        Debug.Log("FeedbackFunc appelé", this);
        if (feedback == null) return;

        if (shakeRoutine != null) StopCoroutine(shakeRoutine);
        shakeRoutine = StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        feedback.AmplitudeGain = shakeAmplitude;
        feedback.FrequencyGain = shakeFrequency;

        yield return new WaitForSecondsRealtime(shakeDuration);

        feedback.AmplitudeGain = 0f;
        feedback.FrequencyGain = 0f;
        shakeRoutine = null;
    }
}