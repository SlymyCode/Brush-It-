using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private Transform cycleObject;
    void Start()
    {
        cycleObject = GetComponent<Transform>();
    }
    
    void Update()
    {
        var cycleObjectRotation = cycleObject.rotation;
        cycleObjectRotation.x += 0.1f;
        
    }
}
