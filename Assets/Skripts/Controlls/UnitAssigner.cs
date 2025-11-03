using System.Collections;
using UnityEngine;

public class UnitAssigner : MonoBehaviour
{
    [SerializeField] private float _assignmentInterval = 3f;

    private UnitControll _unitManager;
    private ResourceHub _resourceHub;
    private Coroutine _assignmentCoroutine;

    public void Initialize(UnitControll unitManager, ResourceHub resourceHub)
    {
        _unitManager = unitManager;
        _resourceHub = resourceHub;

        _assignmentCoroutine = StartCoroutine(ScanForResourcesRoutine());
    }

    private void OnDestroy()
    {
        if (_assignmentCoroutine != null)
            StopCoroutine(_assignmentCoroutine);
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
        }
    }

    private IEnumerator ScanForResourcesRoutine()
    {
        var wait = new WaitForSeconds(_assignmentInterval);

        while (enabled)
        {
            yield return wait;

            AssignUnitsToResources();
        }
    }
}