using TMPro;
using UnityEngine;

public class BaseView : MonoBehaviour
{
    [SerializeField] private Base _base;
    [SerializeField] private TextMeshProUGUI _resourcesText;
    [SerializeField] private TextMeshProUGUI _modeText;
    [SerializeField] private GameObject _flagIndicator;

    private void Start()
    {
        UpdateView();
        _base.ResourcesChanged += OnResourcesChanged;
        _base.BuildingModeChanged += OnBuildingModeChanged;
    }

    private void OnDestroy()
    {
        _base.ResourcesChanged -= OnResourcesChanged;
        _base.BuildingModeChanged -= OnBuildingModeChanged;
    }

    private void OnResourcesChanged(int newResourcesCount)
    {
        UpdateView();
    }

    private void OnBuildingModeChanged(bool isBuildingMode)
    {
        UpdateView();
    }

    private void UpdateView()
    {
        _resourcesText.text = $"Resources: {_base.TotalResources}";

        if (_base.IsBuildingNewBase)
        {
            _modeText.text = "MODE: BUILDING BASE";
            _modeText.color = Color.yellow;

            if (_flagIndicator != null)
                _flagIndicator.SetActive(true);
        }
        else
        {
            _modeText.text = "MODE: CREATING UNITS";
            _modeText.color = Color.green;

            if (_flagIndicator != null)
                _flagIndicator.SetActive(false);
        }
    }
}