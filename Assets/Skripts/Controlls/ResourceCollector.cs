using System.Collections;
using UnityEngine;

public class ResourceCollector : MonoBehaviour
{
    [SerializeField] private float _scanInterval = 3f;

    private UnitControll _unitManager;
    private ResourceHub _resourceHub;
    private Coroutine _scanCoroutine;

    public void Initialize(UnitControll unitManager, ResourceHub resourceHub)
    {
        _unitManager = unitManager;
        _resourceHub = resourceHub;
        _scanCoroutine = StartCoroutine(ScanForResourcesRoutine());
    }

    private void OnDestroy()
    {
        if (_scanCoroutine != null)
            StopCoroutine(_scanCoroutine);
    }

    private void AssignUnitsToResources()
    {
        if (_unitManager.UnitsCount == 0 || _resourceHub == null)
            return;

        var availableUnits = _unitManager.GetAvailableUnits();

        foreach (Unit unit in availableUnits)
        {
            ITakeResource resource = _resourceHub.GetAvailableResource();

            if (resource != null)
                unit.AssignToCollectResource(resource, transform.position);
            else
                break;
        }
    }

    private IEnumerator ScanForResourcesRoutine()
    {
        var wait = new WaitForSeconds(_scanInterval);

        while (enabled)
        {
            yield return wait;

            AssignUnitsToResources();
        }
    }
}