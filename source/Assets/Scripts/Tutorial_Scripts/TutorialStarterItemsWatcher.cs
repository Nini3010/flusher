using UnityEngine;

public class TutorialStarterItemsWatcher : MonoBehaviour
{
    [Header("Exact starter items")]
    public GameObject starterCan;
    public GameObject starterSoap;

    private bool canIn = false;
    private bool soapIn = false;
    private bool fired = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == starterCan)
            canIn = true;
        if (other.gameObject == starterSoap)
            soapIn = true;

        TryFire();
    }

    private void TryFire()
    {
        if (fired) return;
        if (!canIn || !soapIn) return;
        if (TutorialManager.Instance == null) return;

        fired = true;
        TutorialManager.Instance.OnStarterItemsGrabbed();
    }
}