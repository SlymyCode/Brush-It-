using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Image staminaBar;
    public PlayerMovement player;
    private PlayerControls playerControls;
    [SerializeField] private Animator animator;
    
    [SerializeField] private float drainRate = 0.25f;
    [SerializeField] private float regenRate = 0.20f;
    [SerializeField] private float regenWhileHoldingRate = 0.10f;
    [SerializeField] private float growthRate = 0.5f;
    
    [SerializeField] private float runThreshold = 0.40f;
    [SerializeField] private float stopThreshold = 0.042f;
    [SerializeField] private float cooldownSeconds = 2f;

    public bool CanRun { get; private set; } = true;
    public float Stamina => staminaBar.fillAmount;
    
    private bool isCooldown = false;
    private Coroutine cooldownCoroutine;

    private bool wantsToRun = false;
    
    public void OnRun(InputAction.CallbackContext context)
    {
        wantsToRun = context.ReadValueAsButton();
    }
    
    void Update()
    {
        bool pressingShift = wantsToRun;
        
        if (isCooldown)
        {
            float regen = pressingShift ? regenWhileHoldingRate : regenRate;
            Regenerate(regen);
            return;
        }
        
        if (CanRun && pressingShift && Stamina > stopThreshold && player.IsMoving && animator.GetLayerWeight(1) == 0 && animator.GetLayerWeight(2) == 0)
        {
            Drain();
            
            if (Stamina <= stopThreshold)
            {
                CanRun = false;
                StartCooldownIfNeeded();
            }
        }
        else
        {
            Regenerate(regenRate);
            
            if (!CanRun && Stamina >= runThreshold)
            {
                CanRun = true;
            }
        }

        staminaBar.fillAmount = Mathf.Clamp01(staminaBar.fillAmount);
    }

    void Drain()
    {
        staminaBar.fillAmount -= drainRate * Time.deltaTime;
    }

    void Regenerate(float regenAmount)
    {
        staminaBar.fillAmount += regenAmount * Time.deltaTime;
        staminaBar.fillAmount *= math.exp(growthRate * Time.deltaTime);
    }

    void StartCooldownIfNeeded()
    {
        if (cooldownCoroutine == null)
            cooldownCoroutine = StartCoroutine(CooldownCoroutine());
    }

    IEnumerator CooldownCoroutine()
    {
        isCooldown = true;
        
        float timer = 0f;
        while (timer < cooldownSeconds)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        CanRun = true;
        isCooldown = false;
        cooldownCoroutine = null;
    }
}