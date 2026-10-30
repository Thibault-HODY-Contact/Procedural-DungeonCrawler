using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MasterVolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider slider;

    private void Start()
    {
        float volume = PlayerPrefs.GetFloat("MasterVolume", 1f);

        slider.value = volume;
        SetVolume(volume);

        slider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float value)
    {
        value = Mathf.Clamp(value, 0.0001f, 1f);

        audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);

        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
    }
}