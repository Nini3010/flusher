using System.Collections;
using UnityEngine;

public class FlushLever : MonoBehaviour
{
    [Header("Refs")]
    public ToiletController toilet;
    public string handTag = "Hand";

    [Header("Animation")]
    public float downAngle = 60f;      // End rotation on Z
    public float pressTime = 0.15f;    // Time down AND time up

    private bool isAnimating;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(handTag)) return;
        Press();
    }

    public void Press()
    {
        if (!isAnimating)
            StartCoroutine(PressRoutine());
    }

    private IEnumerator PressRoutine()
    {
        isAnimating = true;

        // cache start/end
        float startZ = transform.localEulerAngles.z;
        float endZ = downAngle;

        // --- move DOWN ---
        float t = 0f;
        while (t < pressTime)
        {
            t += Time.deltaTime;
            float k = t / pressTime;
            float currentZ = Mathf.Lerp(startZ, endZ, k);
            Vector3 rot = transform.localEulerAngles;
            rot.z = currentZ;
            transform.localEulerAngles = rot;
            yield return null;
        }

        // --- move UP ---
        t = 0f;
        while (t < pressTime)
        {
            t += Time.deltaTime;
            float k = t / pressTime;
            float currentZ = Mathf.Lerp(endZ, 0f, k);
            Vector3 rot = transform.localEulerAngles;
            rot.z = currentZ;
            transform.localEulerAngles = rot;
            yield return null;
        }

        isAnimating = false;

        // Trigger the actual flush
        if (toilet != null)
            toilet.Flush();
    }
}