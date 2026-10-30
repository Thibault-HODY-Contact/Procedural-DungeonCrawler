using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MusicVolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider slider;

    private void Start()
    {
        float volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        slider.value = volume;
        SetVolume(volume);

        slider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float value)
    {
        value = Mathf.Max(value, 0.0001f);

        audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);

        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }
}