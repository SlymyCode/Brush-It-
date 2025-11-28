using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ParticleSystemHandler : MonoBehaviour
{
    [SerializeField] private new ParticleSystem particleSystem;
    public StaminaBar stamina;
    public PlayerMovement player;
    private bool shiftPressed = false;
    
    IEnumerator ParticleEmission()
    {
        particleSystem.Emit(1);
        yield return new WaitForSeconds(0.25f);
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        shiftPressed = context.ReadValueAsButton();
    }
    
    private void Update()
    {
        if (shiftPressed && stamina.CanRun && player.IsMoving)
        {
            StartCoroutine(ParticleEmission());
        }
    }
}
