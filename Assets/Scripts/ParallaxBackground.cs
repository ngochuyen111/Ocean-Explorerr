using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform cameraTransform;
    public float parallaxEffect = 0.2f;

    private Vector3 startPosition;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        startPosition = transform.position;
    }

    void LateUpdate()
    {
        float x = cameraTransform.position.x * parallaxEffect;

        transform.position = new Vector3(
            startPosition.x + x,
            startPosition.y,
            startPosition.z
        );
    }
}