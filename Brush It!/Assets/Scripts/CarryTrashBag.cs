using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarryTrashBag : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject trashBag;
    [SerializeField] private List<GameObject> bagsInCart;
    [SerializeField] private Animator animator;
    [SerializeField] private float transitionTime = 0.5f;

    private Coroutine weightRoutine;
    private bool inTrigger;
    public bool carryingBag;
    private int bagIndex = 0;
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
        if (inTrigger && carryingBag && (Keyboard.current.fKey.wasPressedThisFrame || Gamepad.current?.buttonSouth.wasPressedThisFrame == true))
        {
            carryingBag = false;
            trashBag.SetActive(false);
            StartWeightTransition(0);
            bagsInCart[bagIndex].SetActive(true);
            bagIndex++;
        }
        else if (carryingBag)
        {
            trashBag.SetActive(true);
            StartWeightTransition(1);
        }
    }

    public void carryTrashBag()
    {
        carryingBag = true;
    }
    
    private void StartWeightTransition(float target)
    {
        if (weightRoutine != null)
            StopCoroutine(weightRoutine);

        weightRoutine = StartCoroutine(TransitionLayerWeight(target));
    }

    private IEnumerator TransitionLayerWeight(float targetWeight)
    {
        float startWeight = animator.GetLayerWeight(2);
        float timer = 0f;

        while (timer < transitionTime)
        {
            timer += Time.deltaTime;
            float t = timer / transitionTime;

            float newWeight = Mathf.Lerp(startWeight, targetWeight, t);
            animator.SetLayerWeight(2, newWeight);

            yield return null;
        }

        animator.SetLayerWeight(2, targetWeight);
    }
}
