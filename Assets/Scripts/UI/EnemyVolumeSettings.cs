using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class EnemyVolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider slider;

    private void Start()
    {
        float volume = PlayerPrefs.GetFloat("EnemyVolume", 1f);
        slider.value = volume;
        SetVolume(volume);

        slider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float value)
    {
        audioMixer.SetFloat("EnemyVolume", Mathf.Log10(value) * 20);

        PlayerPrefs.SetFloat("EnemyVolume", value);
        PlayerPrefs.Save();
    }
}