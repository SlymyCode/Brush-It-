using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class LevelMusic : MonoBehaviour
{
    [Header("Music Tracks")]
    [SerializeField] private AudioClip[] tracks;

    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioMixerGroup groupA;
    [SerializeField] private AudioMixerGroup groupB;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 2f;

    private AudioSource sourceA;
    private AudioSource sourceB;
    private AudioSource activeSource;
    private AudioSource nextSource;

    private string paramA = "MusicA_Vol";
    private string paramB = "MusicB_Vol";

    private void Awake()
    {
        sourceA = gameObject.AddComponent<AudioSource>();
        sourceB = gameObject.AddComponent<AudioSource>();

        sourceA.outputAudioMixerGroup = groupA;
        sourceB.outputAudioMixerGroup = groupB;

        sourceA.loop = true;
        sourceB.loop = true;

        activeSource = sourceA;
        nextSource = sourceB;

        mixer.SetFloat(paramA, -80f);
        mixer.SetFloat(paramB, -80f);
    }

    private void Start()
    {
        if (tracks.Length > 0)
        {
            activeSource.clip = tracks[0];
            activeSource.Play();
            StartCoroutine(FadeIn(paramA));
        }
    }
    
    public void PlayTrack(int index)
    {
        if (index < 0 || index >= tracks.Length) return;
        
        (activeSource, nextSource) = (nextSource, activeSource);

        (paramA, paramB) = (paramB, paramA);

        activeSource.clip = tracks[index];
        activeSource.Play();

        StopAllCoroutines();
        StartCoroutine(Crossfade(paramB, paramA));
    }

    public void StartFadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(FadeIn(paramA));
    }

    public void StartFadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut(paramA));
    }
    
    private IEnumerator FadeIn(string mixerParam)
    {
        float t = 0f;
        float start = -80f;
        float end = 0f;

        mixer.SetFloat(mixerParam, start);

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float v = Mathf.Lerp(start, end, t / fadeDuration);
            mixer.SetFloat(mixerParam, v);
            yield return null;
        }
    }

    private IEnumerator FadeOut(string mixerParam)
    {
        float t = 0f;
        mixer.GetFloat(mixerParam, out float start);
        float end = -80f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float v = Mathf.Lerp(start, end, t / fadeDuration);
            mixer.SetFloat(mixerParam, v);
            yield return null;
        }
    }

    private IEnumerator Crossfade(string fromParam, string toParam)
    {
        float t = 0f;

        mixer.SetFloat(toParam, -80f);
        mixer.SetFloat(fromParam, 0f);

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float lerp = t / fadeDuration;

            mixer.SetFloat(fromParam, Mathf.Lerp(0f, -80f, lerp));
            mixer.SetFloat(toParam, Mathf.Lerp(-80f, 0f, lerp));

            yield return null;
        }
    }
}