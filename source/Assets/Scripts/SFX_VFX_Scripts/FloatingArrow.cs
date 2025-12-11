using UnityEngine;

public class FloatingArrow : MonoBehaviour
{
    public float floatAmplitude = 0.05f;
    public float floatSpeed = 2f;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.localPosition;
    }

    private void Update()
    {
        float offset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.localPosition = startPos + new Vector3(0, offset, 0);
    }
}