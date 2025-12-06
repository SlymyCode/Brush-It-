using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeliverTrash : MonoBehaviour
{
    [SerializeField] private CleanTrashEvent cleanTrashEvent;
    [SerializeField] private NPCInteraction npc;
    [SerializeField] private List<GameObject> trashBags;
    [SerializeField] ParticleSystem trashBagsParticle;

    private bool inTrigger;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inTrigger = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inTrigger = false;
        }
    }

    private void Update()
    {
        if (cleanTrashEvent.trashBagsCollected == 5 && inTrigger && npc.levelComplete == false)
        {
            foreach (GameObject trashBag in trashBags)
            {
                trashBag.SetActive(false);
            }
            npc.levelComplete = true;
            trashBagsParticle.Emit(40);
        }
    }
}