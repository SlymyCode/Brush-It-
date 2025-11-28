using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class CleanTrashEvent : MonoBehaviour
{
    public Image circleFill;           
    public RectTransform circleAnim;   
    public TextMeshProUGUI keyText;    
    
    public int minRounds = 3;
    public int maxRounds = 7;

    private int targetRounds;
    private int currentRounds;
    
    public float popScale = 1.2f;
    public float popDuration = 0.12f;
    public float shakeAmount = 12f;
    public float shakeDuration = 0.1f;
    
    public AudioSource audioSource;
    
    public AudioClip[] correctSounds;
    public AudioClip[] wrongSounds;
    
    private int correctIndex = 0;
    private int wrongIndex = 0;


    private Key currentKey;

    bool inputEnabled = true;

    public System.Action OnSuccess;
    public System.Action OnFail;

    private Key[] validKeys = { Key.R, Key.T, Key.Y, Key.U };
    
    void Start()
    {
        StartNewSession();
    }

    void Update()
    {
        if (!inputEnabled) return;
        
        foreach (var k in validKeys)
        {
            if (Keyboard.current[k].wasPressedThisFrame)
            {
                if (k == currentKey)
                    CorrectInput();
                else
                    WrongInput();

                return;
            }
        }
    }
    
    void StartNewSession()
    {
        correctIndex = 0;
        targetRounds = Random.Range(minRounds, maxRounds + 1);
        currentRounds = 0;
        circleFill.fillAmount = 0f;
        NextKey();
    }
    
    public void StartQTEFromTrigger()
    {
        StartNewSession();
        gameObject.SetActive(true);
        inputEnabled = true;
    }
    
    void NextKey()
    {
        Key randomKey = validKeys[Random.Range(0, validKeys.Length)];
        currentKey = randomKey;
        keyText.text = randomKey.ToString();
    }

    void CorrectInput()
    {
        currentRounds++;
        PlaySound(correctSounds, ref correctIndex);
        StartCoroutine(PopEffect());

        float fill = (float)currentRounds / targetRounds;
        circleFill.fillAmount = fill;

        if (currentRounds >= targetRounds)
        {
            inputEnabled = false;
            OnSuccess?.Invoke();
            correctIndex = 0;
            wrongIndex = 0;
            return;
        }

        NextKey();
    }
    
    void WrongInput()
    {
        correctIndex = 0;
        PlaySound(wrongSounds, ref wrongIndex);
        StartCoroutine(ShakeEffect());
        StartNewSession();
    }

    void PlaySound(AudioClip[] list, ref int index)
    {
        audioSource.PlayOneShot(list[index]);

        index++;
        if (index >= list.Length)
            index = 0;
    }

    IEnumerator PopEffect()
    {
        inputEnabled = false;

        float t = 0;
        while (t < popDuration)
        {
            t += Time.deltaTime;
            float s = Mathf.Lerp(1f, popScale, t / popDuration);
            circleAnim.localScale = new Vector3(s, s, 1);
            yield return null;
        }

        t = 0;
        while (t < popDuration)
        {
            t += Time.deltaTime;
            float s = Mathf.Lerp(popScale, 1f, t / popDuration);
            circleAnim.localScale = new Vector3(s, s, 1);
            yield return null;
        }

        inputEnabled = true;
    }
    
    IEnumerator ShakeEffect()
    {
        Vector2 original = circleAnim.anchoredPosition;

        float timer = 0f;
        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;
            float x = Random.Range(-shakeAmount, shakeAmount);
            float y = Random.Range(-shakeAmount, shakeAmount);
            circleAnim.anchoredPosition = original + new Vector2(x, y);
            yield return null;
        }
        
        circleAnim.anchoredPosition = original;
    }
    
    public void EndQTE()
    {
        gameObject.SetActive(false);
        inputEnabled = false;
    }
}