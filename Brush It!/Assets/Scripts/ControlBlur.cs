using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ControlBlur : MonoBehaviour
{
    public Volume globalVolume;
    private DepthOfField dof;

    private bool toggleDof;
    public float focalDistance;

    private void ToggleBackground()
    {
        toggleDof = !toggleDof;
        if (globalVolume.profile.TryGet(out dof))
        {
            dof.active = toggleDof;
            dof.focusDistance.value = focalDistance;
        }
    }
    
    void Update()
    {
        if (Keyboard.current[Key.Escape].wasPressedThisFrame)
        {
            ToggleBackground();
        }
    }
}
