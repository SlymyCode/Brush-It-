using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class CleanTrashEvent : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] private float angleOffset = 0f;
    [SerializeField] private Key key = Key.E;
    [SerializeField] private int maxHitsToWin = 3;
    private int hitsToWin = 0;
    [SerializeField] private int maxFails = 3;

    [Header("Speed")]
    [SerializeField] private float baseSpeed = 100f;
    [SerializeField] private float maxSpeed = 300f;
    [SerializeField] private float speedIncrease = 25f;

    [Header("Safe Zone")]
    [SerializeField, Range(0f, 1f)] private float initialZoneSize = 0.25f;
    [SerializeField, Range(0f, 1f)] private float minZoneSize = 0.05f;
    [SerializeField] private float zoneShrink = 0.05f;

    [Header("UI Objects")] 
    [SerializeField] private RectTransform rotatingGroup;
    [SerializeField] private Image rotatingBar;
    [SerializeField] private Image successZone;
    [SerializeField] private TextMeshProUGUI counterText;
    [SerializeField] private GameObject targetObject;

    private float currentAngle;
    private float speed;
    private int hits;
    private int fails;
    private float zoneStart;
    private float currentZoneSize;
    private bool active;

    void Start()
    {
        ResetQTE();
    }

    void Update()
    {
        if (!active) return;
        
        currentAngle += speed * Time.deltaTime;
        currentAngle %= 360f;
        rotatingGroup.localEulerAngles = new Vector3(0, 0, -currentAngle);

        if (Keyboard.current[key].wasPressedThisFrame)
        {
            float barWorldZ = rotatingBar.rectTransform.eulerAngles.z;
            float barAngle = Mathf.Repeat(-barWorldZ + angleOffset, 360f);
            
            float zoneCenter = Mathf.Repeat(zoneStart + (currentZoneSize * 360f) / 2f, 360f);
            
            float diff = Mathf.Abs(Mathf.DeltaAngle(barAngle, zoneCenter));
            
            float halfZoneAngle = (currentZoneSize * 360f) / 2f;

            if (diff <= halfZoneAngle)
            {
                Success();
            }
            else
            {
                Fail();
            }
        }
    }

    void Success()
    {
        hits++;
        
        speed = Mathf.Min(speed + speedIncrease, maxSpeed);
        
        currentZoneSize = Mathf.Max(currentZoneSize - zoneShrink, minZoneSize);
        
        zoneStart = Random.Range(0f, 360f);
        UpdateSuccessZone();
        UpdateCounter();

        if (hits >= hitsToWin)
        {
            if (targetObject) targetObject.SetActive(false);
            EndQTE();
        }
    }

    void Fail()
    {
        fails++;
        if (fails >= maxFails)
        {
            ResetQTE();
        }
    }

    void UpdateCounter()
    {
        if (counterText)
            counterText.text = $"{hits}/{hitsToWin}";
    }

    void ResetQTE()
    {
        hits = 0;
        fails = 0;
        speed = baseSpeed;
        currentAngle = 0;
        currentZoneSize = initialZoneSize;
        zoneStart = Random.Range(0f, 360f);
        UpdateSuccessZone();
        UpdateCounter();
    }

    void UpdateSuccessZone()
    {
        if (!successZone) return;
        
        successZone.fillMethod = Image.FillMethod.Radial360;
        successZone.fillOrigin = 0;
        successZone.fillAmount = currentZoneSize-0.05f;
        successZone.rectTransform.localEulerAngles = new Vector3(0, 0, -zoneStart);
    }

    public void StartQTE(GameObject target)
    {
        hitsToWin = Random.Range(1, maxHitsToWin+1);
        targetObject = target;
        ResetQTE();
        gameObject.SetActive(true);
        active = true;
    }

    public void EndQTE()
    {
        active = false;
        gameObject.SetActive(false);
    }
}
