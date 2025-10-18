using UnityEngine;

public class LookAtObject : MonoBehaviour
{
    [SerializeField] GameObject objectToLookAt;
    
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - objectToLookAt.transform.position);
    }
}