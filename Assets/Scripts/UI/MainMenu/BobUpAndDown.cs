using UnityEngine;

public class BobUpAndDown : MonoBehaviour
{
    public float bobAmount = 0.5f;
    public float bobSpeed = 1f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.localPosition;
    }

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        transform.localPosition = new Vector3(startPosition.x, newY, startPosition.z);
    }
}   