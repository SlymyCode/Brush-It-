using UnityEngine;
using UnityEngine.Audio;

public class MusicVolumeMixer : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    private float baseVol;
    private float userVol;

    void Update()
    {
        mixer.GetFloat("MusicBase", out baseVol);
        mixer.GetFloat("MusicUser", out userVol);

        mixer.SetFloat("MusicVolume", baseVol + userVol);
    }
}
