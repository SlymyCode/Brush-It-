using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrashTrigger : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    
    [SerializeField] private CleanTrashEvent miniGame;
    [SerializeField] private GameObject[] trashObjects;

    private bool playerInTrigger;
    private bool qteActive;
    private bool pressed = false;
    
    [SerializeField] private ParticleSystem particles;
    public Collider col;
    
    private void Start()
    {
        col = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
            playerInTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerInTrigger = false;
        
        if (qteActive)
        {
            UnsubscribeFromMiniGame();
            miniGame.EndQTE();
            qteActive = false;
        }
    }

    public void OnTick(InputAction.CallbackContext context)
    {
        pressed = context.ReadValueAsButton();
    }
    
    void Update()
    {
        if (!playerInTrigger) return;

        if (pressed && !qteActive)
        {
            qteActive = true;
            SubscribeToMiniGame();
            miniGame.gameObject.SetActive(true);
            miniGame.StartQTEFromTrigger();
        }
    }

    private void SubscribeToMiniGame()
    {
        miniGame.OnSuccess += HandleSuccess;
        miniGame.OnFail += HandleFail;
    }

    private void UnsubscribeFromMiniGame()
    {
        miniGame.OnSuccess -= HandleSuccess;
        miniGame.OnFail -= HandleFail;
    }

    private void HandleSuccess()
    {
        SpawnDust();
        StartCoroutine(DisableObjects());
    
        UnsubscribeFromMiniGame();
        miniGame.gameObject.SetActive(false);
        qteActive = false;
    }

    IEnumerator DisableObjects()
    {
        yield return new WaitForSeconds(0.05f);
        
        foreach (var obj in trashObjects)
        {
            Collider[] cols3D = obj.GetComponentsInChildren<Collider>(true);
            foreach (var c in cols3D) c.enabled = false;
            
            obj.SetActive(false);
        }
        
        Collider trigger = GetComponent<Collider>();
        if (trigger != null) trigger.enabled = false;
        else
        {
            var any3d = GetComponentInChildren<Collider>();
            if (any3d != null) any3d.enabled = false;
        }
        
        pressed = false;
        playerInTrigger = false;

        yield break;
    }
    
    private void HandleFail()
    {
        UnsubscribeFromMiniGame();
        miniGame.gameObject.SetActive(false);
        qteActive = false;
    }

    private void OnDisable()
    {
        if (miniGame != null)
        {
            UnsubscribeFromMiniGame();
        }
    }
    
    private void SpawnDust()
    {
        particles.Emit(120);
    }
}