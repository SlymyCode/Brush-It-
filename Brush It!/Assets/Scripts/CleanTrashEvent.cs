using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Random = UnityEngine.Random;
using TMPro;

public class CleanTrashEvent : MonoBehaviour
{
    public Image circleFill; 
    public RectTransform circleAnim;
    
    public bool controllerMode = false;
    
    public Image keyImage;
    
    public Sprite spriteR;
    public Sprite spriteT;
    public Sprite spriteY;
    public Sprite spriteU;
    
    public Sprite spriteX;
    public Sprite spriteY_btn;
    public Sprite spriteA;
    public Sprite spriteB;
    
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


    private Key currentKeyKeyboard;
    private GamepadButton currentKeyGamepad;

    bool inputEnabled = true;

    public System.Action OnSuccess;
    public System.Action OnFail;
    
    private Key[] validKeysKeyboard = { Key.R, Key.T, Key.Y, Key.U };
    
    private GamepadButton[] validButtonsGamepad = {
        GamepadButton.North,
        GamepadButton.West,
        GamepadButton.South,
        GamepadButton.East
    };
    
    private Dictionary<Key, Sprite> keySpritesKeyboard;
    private Dictionary<GamepadButton, Sprite> keySpritesGamepad;
    
    private Coroutine shakeRoutine;
    Vector2 original;
    public Toggle controllerToggle;
    
    [SerializeField] private TextMeshProUGUI trashBagsCount;
    public int trashBagsCollected;
    
    public void ControllerModeEnabled()
    {
        controllerMode = !controllerMode;
    }
    
    void Awake()
    {
        keySpritesKeyboard = new Dictionary<Key, Sprite>()
        {
            { Key.R, spriteR },
            { Key.T, spriteT },
            { Key.Y, spriteY },
            { Key.U, spriteU },
        };

        keySpritesGamepad = new Dictionary<GamepadButton, Sprite>()
        {
            { GamepadButton.West,  spriteX },
            { GamepadButton.North, spriteY_btn },
            { GamepadButton.South, spriteA },
            { GamepadButton.East,  spriteB },
        };
    }

    void Update()
    {
        if (!inputEnabled) return;

        if (!controllerMode)
        {
            foreach (var key in validKeysKeyboard)
            {
                if (Keyboard.current[key].wasPressedThisFrame)
                {
                    if (key == currentKeyKeyboard)
                        CorrectInput();
                    else
                        WrongInput();
                    return;
                }
            }
        }
        else if (Gamepad.current != null)
        {
            foreach (var btn in validButtonsGamepad)
            {
                if (Gamepad.current[btn].wasPressedThisFrame)
                {
                    if (btn == currentKeyGamepad)
                        CorrectInput();
                    else
                        WrongInput();
                    return;
                }
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
    if (!controllerMode)
    {
        Key randomKey = validKeysKeyboard[Random.Range(0, validKeysKeyboard.Length)];
        currentKeyKeyboard = randomKey;
        keyImage.sprite = keySpritesKeyboard[randomKey];
    }
    else
    {
        GamepadButton randomBtn = validButtonsGamepad[Random.Range(0, validButtonsGamepad.Length)];
        currentKeyGamepad = randomBtn;
        keyImage.sprite = keySpritesGamepad[randomBtn];
    }
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
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(DoShake());
        yield return null;
    }

    IEnumerator DoShake()
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

        shakeRoutine = null;
    }
    
    public void EndQTE()
    {
        gameObject.SetActive(false);
        inputEnabled = false;
    }

    public void UpdateCounter()
    {
        trashBagsCollected += 1;
        trashBagsCount.text = $"{trashBagsCollected}/5";  
    }
}