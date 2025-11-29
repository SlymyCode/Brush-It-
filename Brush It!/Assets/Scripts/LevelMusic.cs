using UnityEngine;
using UnityEngine.Audio;

public class LevelMusic : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip song;
    [SerializeField] private AudioMixerGroup mixer;

    public float fadeDuration = 3f; // tiempo del fade in en segundos

    private void Start()
    {
        audioSource.loop = true;
        audioSource.clip = song;
        audioSource.outputAudioMixerGroup = mixer;

        // Ponemos la música en silencio (-80 dB es prácticamente mute)
        mixer.audioMixer.SetFloat("MusicVolume", -10f);

        audioSource.Play();

        // Empezamos el fade in
        StartCoroutine(FadeIn());
    }

    private System.Collections.IEnumerator FadeIn()
    {
        float currentTime = 0f;
        float startVolume = -25f;   // silencio
        float targetVolume = -10f;    // volumen normal

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(startVolume, targetVolume, currentTime / fadeDuration);
            mixer.audioMixer.SetFloat("MusicVolume", newVolume);
            yield return null;
        }
    }
}