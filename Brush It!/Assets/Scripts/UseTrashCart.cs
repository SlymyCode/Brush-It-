using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class UseTrashCart : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject cart;
    [SerializeField] private Transform playerAttachPoint;
    [SerializeField] private Animator animator;
    [SerializeField] private float transitionTime = 0.5f;
    [SerializeField] private CarryTrashBag carryTrashBag;
    
    private Coroutine weightRoutine;
    private bool inTrigger;
    private bool usingCart;
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
        if (usingCart && (Keyboard.current.eKey.wasPressedThisFrame || Gamepad.current?.buttonWest.wasPressedThisFrame == true))
        {
            DetachPlayer();
            usingCart = false;
        }
        else if (inTrigger && (Keyboard.current.eKey.wasPressedThisFrame || Gamepad.current?.buttonWest.wasPressedThisFrame == true) && !carryTrashBag.carryingBag)
        {
            AttachPlayer();
            usingCart = true;
        }
        else if (usingCart)
        {
            cart.transform.SetPositionAndRotation(playerAttachPoint.position, playerAttachPoint.rotation);
        }
    }

    private void AttachPlayer()
    {
        cart.transform.SetPositionAndRotation(playerAttachPoint.position, playerAttachPoint.rotation);
            
        cart.transform.SetParent(transform);

        StartWeightTransition(1);
    }

    private void DetachPlayer()
    {
        cart.transform.SetParent(null);
        StartWeightTransition(0);
    }
    
    private void StartWeightTransition(float target)
    {
        if (weightRoutine != null)
            StopCoroutine(weightRoutine);

        weightRoutine = StartCoroutine(TransitionLayerWeight(target));
    }

    private IEnumerator TransitionLayerWeight(float targetWeight)
    {
        float startWeight = animator.GetLayerWeight(1);
        float timer = 0f;

        while (timer < transitionTime)
        {
            timer += Time.deltaTime;
            float t = timer / transitionTime;

            float newWeight = Mathf.Lerp(startWeight, targetWeight, t);
            animator.SetLayerWeight(1, newWeight);

            yield return null;
        }

        animator.SetLayerWeight(1, targetWeight);
    }
}