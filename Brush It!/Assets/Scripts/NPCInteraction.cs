using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class NPCInteraction : MonoBehaviour
{
    public GameObject accept;
    public GameObject cancel;
    public GameObject textContainer;
    public TextMeshProUGUI text;
    public string[] lines;
    public string[] finalLines;
    public float textSpeed;
    public List<GameObject> collectibleTrash;
    public List<TrashTrigger> trashDust;
    public Timer timer;
    public LevelMusic levelMusic;
    
    private int index;
    private bool playerInTrigger;
    public bool playing;
    public bool levelComplete;
    public bool gameFinished;
    private bool talking;

    void Start()
    {
        textContainer.SetActive(false);
        accept.SetActive(false);
        cancel.SetActive(false);
        text.text = "";
    }

    void Update()
    {
        if (timer.remainingTime <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Time.timeScale = 1;
        }
        
        if ((Keyboard.current.eKey.wasPressedThisFrame || Gamepad.current?.buttonWest.wasPressedThisFrame == true) && playerInTrigger && !playing && !talking)
        {
            talking = true;
            textContainer.SetActive(true);
            StartDialogue();
        }
        
        if ((Keyboard.current.eKey.wasPressedThisFrame || Gamepad.current?.buttonWest.wasPressedThisFrame == true) && playerInTrigger && levelComplete && !talking)
        {
            talking = true;
            textContainer.SetActive(true);
            LastDialogue();
            gameFinished = true;
        }
        
        if ((Mouse.current.leftButton.wasPressedThisFrame || Gamepad.current?.buttonSouth.wasPressedThisFrame == true) && textContainer.activeInHierarchy && !levelComplete)
        {
            if (text.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                text.text = lines[index];
            }
        }
        
        if ((Mouse.current.leftButton.wasPressedThisFrame || Gamepad.current?.buttonSouth.wasPressedThisFrame == true) && textContainer.activeInHierarchy && levelComplete)
        {
            if (text.text == finalLines[index])
            {
                NextLineLast();
            }
            else
            {
                StopAllCoroutines();
                text.text = finalLines[index];
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            text.text = String.Empty;
            textContainer.SetActive(false);
            playerInTrigger = false;
            accept.SetActive(false);
            cancel.SetActive(false);
            talking = false;
            StopAllCoroutines();
        }
        
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            text.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            text.text = String.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            accept.SetActive(true);
            cancel.SetActive(true);
            talking = false;
        }
    }

    void LastDialogue()
    {
        index = 0;
        StartCoroutine(TypeLineLast());
    }

    IEnumerator TypeLineLast()
    {
        foreach (char c in finalLines[index].ToCharArray())
        {
            text.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLineLast()
    {
        if (index < finalLines.Length - 1)
        {
            index++;
            text.text = String.Empty;
            StartCoroutine(TypeLineLast());
        }
        else
        {
            text.text = String.Empty;
            textContainer.SetActive(false);
            playerInTrigger = false;
            talking = false;
        }
    }
    
    public void Accept()
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            levelMusic.StartFadeOut();
            SceneManager.LoadScene("Level 1");
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            playing = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            text.text = String.Empty;
            textContainer.SetActive(false);
            playerInTrigger = false;
            accept.SetActive(false);
            cancel.SetActive(false);
            foreach (GameObject trash in collectibleTrash)
            {
                trash.SetActive(true);
            }
            foreach (var dust in trashDust)
            {
                dust.SpawnDust();
            }
        }
    }

    public void Deny()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        text.text = String.Empty;
        textContainer.SetActive(false);
        playerInTrigger = false;
        accept.SetActive(false);
        cancel.SetActive(false);   
    }
}