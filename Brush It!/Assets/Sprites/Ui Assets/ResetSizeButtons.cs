using UnityEngine;

public class ResetSizeButtons : MonoBehaviour
{
    private RectTransform rect;
    private Vector2 baseSize;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        baseSize = rect.sizeDelta;
    }

    public void ResetSize()
    {
        rect.sizeDelta = baseSize;
    }
}
