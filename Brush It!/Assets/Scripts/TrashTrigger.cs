using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrashTrigger : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Key key = Key.E;
    [SerializeField] private string playerTag = "Player";

    [Header("Referencias")]
    [SerializeField] private CleanTrashEvent miniGame;
    [SerializeField] private GameObject[] trashObjects;

    private bool playerInTrigger;
    private bool qteActive;
    
    [SerializeField] private ParticleSystem particles;

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

    void Update()
    {
        if (!playerInTrigger) return;

        if (Keyboard.current[key].wasPressedThisFrame && !qteActive)
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
        StartCoroutine(DisableTrash());
        
        UnsubscribeFromMiniGame();
        miniGame.gameObject.SetActive(false);
        qteActive = false;
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

    IEnumerator DisableTrash()
    {
        yield return new WaitForSeconds(0.2f);

        foreach (var obj in trashObjects)
            if (obj) obj.SetActive(false);
    }
}