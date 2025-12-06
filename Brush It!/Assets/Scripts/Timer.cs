using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private NPCInteraction npc;
    public float remainingTime;
    private bool showTimer;
    private bool timerStarted;
    
    void Start()
    {
        timerStarted = true;
        showTimer = true;
    }
    
    void Update()
    {
        if (npc.playing == true && timerStarted)
        {
            StartCoroutine(StartTimer());
            timerStarted = false;
        }

        if (npc.gameFinished)
        {
            StopAllCoroutines();
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
        
        if (showTimer)
            timerText.text = "00:00";
        else
            timerText.text = "";
    }
}
