using UnityEngine;

public class StartScreen : MonoBehaviour
{
    public GameObject startPanel;
    public TutorialManager tutorialManager;

    private const string TutorialSeenKey = "TutorialSeen";

    private void Start()
    {
        bool seen = PlayerPrefs.GetInt(TutorialSeenKey, 0) == 1;

        if (startPanel != null)
            startPanel.SetActive(!seen);
        
        if (seen && tutorialManager != null)
        {
            tutorialManager.SetTutorialCompleted(true);
        }
    }
    
    public void OnUnderstoodClicked()
    {
        if (startPanel != null)
            startPanel.SetActive(false);

        if (tutorialManager != null)
            tutorialManager.StartTutorial();
    }

    public static void MarkTutorialSeen()
    {
        PlayerPrefs.SetInt(TutorialSeenKey, 1);
        PlayerPrefs.Save();
    }
}