using UnityEngine;
using UnityEngine.InputSystem;

public class TrashTrigger : MonoBehaviour
{
    [SerializeField] private Key key = Key.E;
    [SerializeField] private CleanTrashEvent cte;
    [SerializeField] private string playerTag = "Player";

    private bool playerInTrigger;
    private bool qteActive;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInTrigger = false;
            
            if (qteActive)
            {
                cte.EndQTE();
                qteActive = false;
            }
        }
    }

    void Update()
    {
        if (!playerInTrigger) return;
        
        if (Keyboard.current[key].wasPressedThisFrame && !qteActive)
        {
            qteActive = true;
            cte.StartQTE(gameObject);
        }
    }
}