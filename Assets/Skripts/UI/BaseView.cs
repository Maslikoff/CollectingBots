using TMPro;
using UnityEngine;

public class BaseView : MonoBehaviour
{
    [SerializeField] private Base _base;
    [SerializeField] private TextMeshProUGUI _resourcesText;
    [SerializeField] private TextMeshProUGUI _resourceTypeText;

    private void Start()
    {
        UpdateResourcesText();
        UpdateResourceTypeText();

        _base.ResourcesChanged += OnResourcesChanged;
        _base.ResourceTypeChanged += OnResourceTypeChanged;
    }

    private void OnDestroy()
    {
        _base.ResourcesChanged -= OnResourcesChanged;
        _base.ResourceTypeChanged -= OnResourceTypeChanged;
    }

    private void OnResourcesChanged(int newResourcesCount)
    {
        UpdateResourcesText();
    }

    private void OnResourceTypeChanged(ResourceType newResourceType)
    {
        UpdateResourceTypeText();
    }

    private void UpdateResourcesText()
    {
        _resourcesText.text = $"{_base.TotalResources}";
    }

    private void UpdateResourceTypeText()
    {
        _resourceTypeText.text = $"{_base.AllowedResources}";
    }
}