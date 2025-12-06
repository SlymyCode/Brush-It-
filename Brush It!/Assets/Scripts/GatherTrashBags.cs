using System;
using System.Collections.Generic;
using UnityEngine;

public class GatherTrashBags : MonoBehaviour
{
    [SerializeField] private List<GameObject> trashBags;
    [SerializeField] private float trashBagsCollected;
    
    private void OnTriggerEnter(Collider other)
    {
        
    }
}