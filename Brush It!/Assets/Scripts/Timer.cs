using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class Timer : MonoBehaviour
{
    [SerializeField] Key key = Key.T;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float remainingTime;
    float elapsedTime;
    private bool showTimer;

    void Start()
    {
        showTimer = true;
    }
    
    void Update()
    {
        remainingTime -= Time.deltaTime;
        
        if (Keyboard.current[key].wasPressedThisFrame && showTimer)
        {
            showTimer = false;
        }
        else if (Keyboard.current[key].wasPressedThisFrame && !showTimer)
        {
            showTimer = true;
        }
        
        if (remainingTime >= 0 && showTimer)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else if (remainingTime >= 0 && !showTimer)
        {
            timerText.text = "";
        }
        else if (remainingTime <= 0 && showTimer)
        {
            timerText.text = "00:00";
        }
        else
        {
            timerText.text = "";
        }
    }
}
