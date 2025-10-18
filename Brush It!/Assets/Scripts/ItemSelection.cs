using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ItemSelector : MonoBehaviour
{
    [System.Serializable]
    public class FrameOption
    {
        public Image frame;
        public InputAction input;
    }

    [SerializeField] private List<FrameOption> frames = new List<FrameOption>();
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private float transitionDuration = 0.3f;
    
    private Image currentSelected;
    private Dictionary<Image, Coroutine> activeTransitions = new Dictionary<Image, Coroutine>();

    private void OnEnable()
    {
        foreach (var f in frames)
        {
            f.input.Enable();
            f.input.performed += context => OnFrameSelected(f.frame);
        }
    }

    private void OnDisable()
    {
        foreach (var f in frames)
        {
            f.input.Disable();
        }
    }
    private IEnumerator ColorTransition(Image img, Color targetColor)
    {
        Color startColor = img.color;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            img.color = Color.Lerp(startColor, targetColor, elapsed / transitionDuration);
            yield return null;
        }

        img.color = targetColor;
    }
    private void StartColorTransition(Image img, Color targetColor)
    {
        if (activeTransitions.ContainsKey(img) && activeTransitions[img] != null)
            StopCoroutine(activeTransitions[img]);

        activeTransitions[img] = StartCoroutine(ColorTransition(img, targetColor));
    }
    private void OnFrameSelected(Image selected)
    {
        if (currentSelected != null)
            StartColorTransition(currentSelected, normalColor);
        
        StartColorTransition(selected, selectedColor);
        currentSelected = selected;
    }
}