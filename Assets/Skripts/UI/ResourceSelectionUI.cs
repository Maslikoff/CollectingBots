using UnityEngine;
using UnityEngine.UI;

public class ResourceSelectionUI : MonoBehaviour
{
    [SerializeField] private Button _woodButton;
    [SerializeField] private Button _stoneButton;
    [SerializeField] private Button _metallButton;
    [SerializeField] private GameObject _buttonPanel;

    private Base _base;

    private void Start()
    {
        _base = GetComponent<Base>();

        _woodButton.onClick.AddListener(() => SetResourceType(ResourceType.Wood));
        _stoneButton.onClick.AddListener(() => SetResourceType(ResourceType.Stone));
        _metallButton.onClick.AddListener(() => SetResourceType(ResourceType.Metall));

        UpdateButtonStates();
    }

    public void EnableAllButtons()
    {
        _buttonPanel.SetActive(true);
        UpdateButtonStates();
    }

    public void DisableAllButtons()
    {
        _buttonPanel.SetActive(false);
    }

    private void SetResourceType(ResourceType resourceType)
    {
        _base.SetAllowedResource(resourceType);
        UpdateButtonStates();
        DisableAllButtons();
    }

    private void UpdateButtonStates()
    {
        _woodButton.interactable = true;
        _stoneButton.interactable = true;
        _metallButton.interactable = true;
   
        switch (_base.AllowedResources)
        {
            case ResourceType.Wood:
                _woodButton.interactable = false;
                break;

            case ResourceType.Stone:
                _stoneButton.interactable = false;
                break;

            case ResourceType.Metall:
                _metallButton.interactable = false;
                break;
        }
    }
}