using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRBaseInteractable))]
public class ExitInteractable : MonoBehaviour
{
    [Header("Highlight")]
    public Renderer targetRenderer;
    public Material normalMaterial;
    public Material highlightMaterial;
    
    [Header("UI")]
    public GameObject confirmWindow;
    
    private XRBaseInteractable _interactable;
    
    private void Awake()
    {
        _interactable = GetComponent<XRBaseInteractable>();

        _interactable.hoverEntered.AddListener(OnHoverEntered);
        _interactable.hoverExited.AddListener(OnHoverExited);
        _interactable.selectEntered.AddListener(OnSelected);

        if (confirmWindow != null)
            confirmWindow.SetActive(false);

        if (targetRenderer != null && normalMaterial != null)
            targetRenderer.material = normalMaterial;
    }

    private void OnDestroy()
    {
        if (_interactable == null) return;

        _interactable.hoverEntered.RemoveListener(OnHoverEntered);
        _interactable.hoverExited.RemoveListener(OnHoverExited);
        _interactable.selectEntered.RemoveListener(OnSelected);
    }
    
    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (targetRenderer != null && highlightMaterial != null)
            targetRenderer.material = highlightMaterial;
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        if (targetRenderer != null && normalMaterial != null)
            targetRenderer.material = normalMaterial;
    }

    
    private void OnSelected(SelectEnterEventArgs args)
    {
        if (confirmWindow != null)
        {
            confirmWindow.SetActive(true);
            
            _interactable.enabled = false;
            if(targetRenderer != null && normalMaterial != null)
                targetRenderer.material = normalMaterial;
        }
    }

    public void ConfirmExit() //THE SHEBANG IF STATE IS PURELY DEBUG SO I COULD SHOW IT OFF IN THE UNITY EDITOR
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void CancelExit()
    {
        if (confirmWindow != null)
            confirmWindow.SetActive(false);
        
        if (_interactable != null)
            _interactable.enabled = true;
    }
}
