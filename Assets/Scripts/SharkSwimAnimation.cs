using UnityEngine;

public class SharkSwimAnimation : MonoBehaviour
{
    public float rotateAmount = 6f;
    public float rotateSpeed = 5f;

    private Quaternion startRot;

    void Start()
    {
        startRot = transform.localRotation;
    }

    void Update()
    {
        float z = Mathf.Sin(Time.time * rotateSpeed) * rotateAmount;
        transform.localRotation = startRot * Quaternion.Euler(0, 0, z);
    }
}