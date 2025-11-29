using System;
using UnityEngine;
using UnityEngine.Audio;

public class MixerController : MonoBehaviour
{
    [SerializeField] AudioMixerGroup audioMixer;
    [SerializeField] private string mixerParameter;

    public void SetVolume(float sliderValue)
    {
        audioMixer.audioMixer.SetFloat(mixerParameter, (Mathf.Log10(sliderValue) * 20));
    }
}