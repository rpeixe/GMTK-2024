using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private string mixerParam;
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _slider.onValueChanged.AddListener(SetVolume);
    }

    private void OnEnable()
    {
        _mixer.GetFloat(mixerParam, out float volume);
        _slider.value = Mathf.Pow(10, volume/20);
    }

    private void SetVolume(float value)
    {
        _mixer.SetFloat(mixerParam, Mathf.Log10(value) * 20);
    }
}
