using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("Arrows")]
    public GameObject arrowStall;
    public GameObject arrowCan;
    public GameObject arrowSoap;
    public GameObject arrowHandle;
    public GameObject arrowShop;

    [Header("Timing")]
    public float stallIntroDuration = 20f;
    public float stallHintDelay = 8f;          
    
    [Header("Text UI")] 
    public HUDController hud;

    private enum Step
    {
        Inactive,
        StallIntro,
        GrabItems,
        FlushItems,
        GoToShop,
        BuyFromShop,
        Done
    }

    private Step currentStep = Step.Inactive;
    private float stepTimer = 0f;
    private bool stallHintShown = false;

    // NEW — track tutorial completion for saving/loading
    private bool tutorialFinished = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Always start with nothing visible
        SetAllArrows(false, false, false, false, false);
        SetText("");
    }

    private void Update()
    {
        if (currentStep == Step.Inactive || currentStep == Step.Done)
            return;

        stepTimer += Time.deltaTime;

        switch (currentStep)
        {
            case Step.StallIntro:
                if (stepTimer >= stallIntroDuration)
                    GoToGrabItems();
                break;

            case Step.GoToShop:
                if (!stallHintShown && stepTimer >= stallHintDelay)
                {
                    stallHintShown = true;
                    SetText("Try knocking on the stall door and read the sign.");
                }
                break;
        }
    }

    // -------------------------------------------------------
    // Public API – called from other scripts
    // -------------------------------------------------------

    public void StartTutorial()
    {
        // If tutorial is already completed, do nothing
        if (tutorialFinished)
        {
            SetTutorialCompleted(true); // ensures arrows are off
            return;
        }

        if (currentStep != Step.Inactive) return;

        currentStep = Step.StallIntro;
        stepTimer = 0f;
        stallHintShown = false;

        SetAllArrows(stall: true, can: false, soap: false, handle: false, shop: false);
        SetText("This stall looks suspicious. We’ll come back to it later.");
    }

    public void OnStarterItemsGrabbed()
    {
        if (currentStep != Step.GrabItems) return;
        GoToFlushItems();
    }

    public void OnFirstFlush()
    {
        if (currentStep != Step.FlushItems) return;
        GoToShopStep();
    }

    public void OnShopDoorOpened()
    {
        if (currentStep != Step.GoToShop) return;

        currentStep = Step.BuyFromShop;
        stepTimer = 0f;

        SetAllArrows(false, false, false, false, true);
        SetText("Pick something from the shop and grab it.");
    }

    public void OnFirstShopPurchase()
    {
        if (currentStep != Step.BuyFromShop) return;
        FinishTutorial();
    }

    // -------------------------------------------------------
    // NEW — Save/Load API
    // -------------------------------------------------------

    public bool WasTutorialCompleted()
    {
        return tutorialFinished;
    }

    public void SetTutorialCompleted(bool completed)
    {
        tutorialFinished = completed;

        if (completed)
        {
            currentStep = Step.Done;
            SetAllArrows(false, false, false, false, false);
            SetText(""); // remove tutorial messages
        }
        else
        {
            // If loading a save where tutorial is NOT completed,
            // restart tutorial steps from the beginning:
            currentStep = Step.Inactive;
        }
    }

    // -------------------------------------------------------
    // Internal transitions
    // -------------------------------------------------------

    private void GoToGrabItems()
    {
        currentStep = Step.GrabItems;
        stepTimer = 0f;

        SetAllArrows(false, true, true, false, false);
        SetText("Let’s pick up your first 2 items and throw them into the toilet.");
    }

    private void GoToFlushItems()
    {
        currentStep = Step.FlushItems;
        stepTimer = 0f;

        SetAllArrows(false, false, false, true, false);
        SetText("Nice. Now pull the handle to flush them.");
    }

    private void GoToShopStep()
    {
        currentStep = Step.GoToShop;
        stepTimer = 0f;
        stallHintShown = false;

        SetAllArrows(true, false, false, false, false);
        SetText("With your newfound wealth, let’s buy more stuff at the stall.");
    }

    private void FinishTutorial()
    {
        tutorialFinished = true;
        currentStep = Step.Done;

        SetAllArrows(false, false, false, false, false);
        SetText("Let the flushening begin.");

        StartScreen.MarkTutorialSeen();
    }

    // -------------------------------------------------------
    // Helpers
    // -------------------------------------------------------

    private void SetAllArrows(bool stall, bool can, bool soap, bool handle, bool shop)
    {
        if (arrowStall != null) arrowStall.SetActive(stall);
        if (arrowCan != null) arrowCan.SetActive(can);
        if (arrowSoap != null) arrowSoap.SetActive(soap);
        if (arrowHandle != null) arrowHandle.SetActive(handle);
        if (arrowShop != null) arrowShop.SetActive(shop);
    }

    private void SetText(string msg)
    {
        if (string.IsNullOrEmpty(msg) || hud == null) return;
        hud.ShowDemonLine(msg);
    }
}