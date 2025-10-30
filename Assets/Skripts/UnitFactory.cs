using UnityEngine;

public class UnitFactory : MonoBehaviour
{
    [SerializeField] private int _resourcesForNewUnit = 3;

    private UnitControll _unitControll;
    private ResourceHub _resourceHub;
    private bool _isProducing = false;

    public void Initialize(UnitControll unitManager, ResourceHub resourceHub)
    {
        _unitControll = unitManager;
        _resourceHub = resourceHub;
    }

    public void UpdateProduction()
    {
        if (_isProducing) 
            return;

        if (_resourceHub.TotalResources >= _resourcesForNewUnit)
        {
            if (_resourceHub.TrySpendResources(_resourcesForNewUnit))
            {
                _isProducing = true;
                _unitControll.CreateNewUnit();
                _isProducing = false;
            }
        }
    }
}