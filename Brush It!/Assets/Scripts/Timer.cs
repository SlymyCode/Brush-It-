using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class Timer : MonoBehaviour
{
    [SerializeField] Key key = Key.T;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float remainingTime;
    private bool showTimer;

    void Start()
    {
        showTimer = true;
    }
    
    void Update()
    {
        if (Keyboard.current[key].wasPressedThisFrame)
        {
            StartCoroutine(StartTimer());
        }
    }

    IEnumerator StartTimer()
    {
        remainingTime -= Time.deltaTime;

        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;

            if (showTimer)
            {
                int minutes = Mathf.FloorToInt(remainingTime / 60);
                int seconds = Mathf.FloorToInt(remainingTime % 60);
                timerText.text = $"{minutes:00}:{seconds:00}";
            }
            else
            {
                timerText.text = "";
            }

            yield return null;
        }

        // Cuando llegue a 0
        if (showTimer)
            timerText.text = "00:00";
        else
            timerText.text = "";
    }
}
