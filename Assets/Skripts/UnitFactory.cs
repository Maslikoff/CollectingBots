using UnityEngine;

public class UnitFactory : MonoBehaviour
{
    [SerializeField] private int _resourcesForNewUnit = 3;

    private UnitControll _unitControll;
    private ResourceHub _resourceHub;
    private Base _base;

    private bool _isProducing = false;

    public void Initialize(UnitControll unitManager, ResourceHub resourceHub, Base baseObj)
    {
        _unitControll = unitManager;
        _resourceHub = resourceHub;
        _base = baseObj;
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
                CreateUnitWithDelay();
            }
        }
    }

    private void CreateUnitWithDelay()
    {
        StartCoroutine(ProductionRoutine());
    }

    private System.Collections.IEnumerator ProductionRoutine()
    {
        yield return new WaitForSeconds(1f);

        _unitControll.CreateNewUnit();
        _isProducing = false;
    }
}