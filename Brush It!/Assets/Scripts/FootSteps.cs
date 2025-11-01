using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSteps : MonoBehaviour
{
    [Header("Components")] 
    Animator animator;
    AudioSource audioSource;
    ParticleSystem footstepFx;

    [Header("Settings")]
    public AudioClip[] footstepSounds;
    readonly string animParam_Walking = "walking";
    public bool visualize = false;
    
    [Header("State")] 
    public bool walking;

    void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        footstepFx = GetComponent<ParticleSystem>();
    }
    void Update()
    {
        animator.SetBool(animParam_Walking, walking);
    }

    public void Footstep()
    {
        float movement = animator.GetFloat("MovementStrength");
        if (movement > 0.1f)
        {
            int random = Random.Range(0, footstepSounds.Length);
            var clip = footstepSounds[random];
            audioSource.PlayOneShot(clip);
            float baseVolume = (movement <= 0.51f) ? Random.Range(0.05f, 0.15f) : Random.Range(0.15f, 0.2f);
            StartCoroutine(PlayFootstepWithFade(clip, baseVolume));
            if (visualize)
            {
                footstepFx.Emit(1);
            }
        }
    }
    
    private IEnumerator PlayFootstepWithFade(AudioClip clip, float volume)
    {
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();

        float fadeDuration = 0.2f;
        float startTime = Time.unscaledTime;
        float startVol = volume;

        while (audioSource.isPlaying && Time.unscaledTime - startTime < fadeDuration)
        {
            audioSource.volume = Mathf.Lerp(startVol, 0f, (Time.unscaledTime - startTime) / fadeDuration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVol;
    }
}