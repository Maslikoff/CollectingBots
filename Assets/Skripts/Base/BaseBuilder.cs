using System;
using UnityEngine;

public class BaseBuilder : MonoBehaviour
{
    [SerializeField] private Base _basePrefab;
    [SerializeField] private int _resourcesForNewBase = 5;
    [SerializeField] private int _minUnitsForNewBase = 2;

    private UnitControll _unitControll;
    private ResourceHub _resourceHub;
    private FlagControll _flagController;
    private Base _base;

    public bool IsBuilding { get; private set; }

    public event Action<bool> BuildingModeChanged;

    public void Initialize(UnitControll unitManager, ResourceHub resourceHub, FlagControll flagController, Base baseObj)
    {
        _unitControll = unitManager;
        _resourceHub = resourceHub;
        _flagController = flagController;
        _base = baseObj;
    }

    public void StartBuilding()
    {
        if (_flagController.HasFlag == false)
            return;

        IsBuilding = true;
        BuildingModeChanged?.Invoke(true);
    }

    public void StopBuilding()
    {
        IsBuilding = false;
        BuildingModeChanged?.Invoke(false);
    }

    public void UpdateBuilding()
    {
        if (IsBuilding == false || _flagController.HasFlag == false)
            return;

        if (_resourceHub.TotalResources >= _resourcesForNewBase && _unitControll.UnitsCount > _minUnitsForNewBase)
            if (_resourceHub.TrySpendResources(_resourcesForNewBase))
                SendUnitToBuildBase();
    }

    private void SendUnitToBuildBase()
    {
        Unit builder = _unitControll.GetAvailableUnit();

        if (builder == null)
            return;

        _unitControll.RemoveUnit(builder);

        builder.BuildNewBase(_flagController.FlagPosition, OnNewBaseBuilt);
    }

    private void OnNewBaseBuilt(Unit builder)
    {
        if (_basePrefab == null && builder == null)
        {
            StopBuilding();
            return;
        }

        Vector3 buildPosition = builder.transform.position;
        buildPosition.y = 0f;

        GameObject newBaseObject = Instantiate(_basePrefab.gameObject, builder.transform.position, Quaternion.identity);
        Base newBase = newBaseObject.GetComponent<Base>();

        if (newBase == null)
        {
            Destroy(newBaseObject);
            StopBuilding();

            return;
        }

        Unit[] allUnits = newBase.GetComponentsInChildren<Unit>(true);

        foreach (Unit unit in allUnits)
            unit.gameObject.SetActive(false);

        newBase.SetAsNewBase();
        newBase.AddUnit(builder);
        builder.SetBase(newBase);

        _flagController.RemoveFlag();
        StopBuilding();
    }
}