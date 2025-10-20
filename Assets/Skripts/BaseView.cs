using TMPro;
using UnityEngine;

public class BaseView : MonoBehaviour
{
    [SerializeField] private Base _base;
    [SerializeField] private TextMeshProUGUI _resourcesText;

    private void Start()
    {
        _resourcesText.text = _base.TotalResources.ToString();

        _base.ResourcesChanged += OnResourcesChanged;
        OnResourcesChanged(_base.TotalResources);
    }

    private void OnDestroy()
    {
        _base.ResourcesChanged -= OnResourcesChanged;
    }

    private void OnResourcesChanged(int newResourcesCount)
    {
        _resourcesText.text = newResourcesCount.ToString();
        _resourcesText.text = _base.TotalResources.ToString();
    }
}