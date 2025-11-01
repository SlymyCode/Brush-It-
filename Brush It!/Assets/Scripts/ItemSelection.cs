using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ItemSelection : MonoBehaviour
{
    [System.Serializable]
    public class FrameOption
    {
        public Image frame;
        public InputAction input;
    }

    [SerializeField] private List<FrameOption> frames;
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private float transitionTime = 0.3f;

    private Image currentSelected;

    private void OnEnable()
    {
        foreach (var f in frames)
        {
            f.input.Enable();
            f.input.performed += ctx => SelectFrame(f.frame);
        }
    }

    private void OnDisable()
    {
        foreach (var f in frames)
        {
            f.input.Disable();
            f.input.performed -= ctx => SelectFrame(f.frame);
        }
    }

    private void SelectFrame(Image selected)
    {
        if (currentSelected == selected)
        {
            StartCoroutine(ChangeColor(selected, normalColor));
            currentSelected = null;
            return;
        }
        
        if (currentSelected != null)
            StartCoroutine(ChangeColor(currentSelected, normalColor));
        
        StartCoroutine(ChangeColor(selected, selectedColor));
        currentSelected = selected;
    }

    private IEnumerator ChangeColor(Image img, Color target)
    {
        Color start = img.color;
        float elapsed = 0f;

        while (elapsed < transitionTime)
        {
            elapsed += Time.deltaTime;
            img.color = Color.Lerp(start, target, elapsed / transitionTime);
            yield return null;
        }

        img.color = target;
    }
}