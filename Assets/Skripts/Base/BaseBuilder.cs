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

    public bool IsBuilding { get; private set; }

    public event Action<bool> BuildingModeChanged;

    public void Initialize(UnitControll unitManager, ResourceHub resourceHub, FlagControll flagController)
    {
        _unitControll = unitManager;
        _resourceHub = resourceHub;
        _flagController = flagController;
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

        builder.BuildNewBase(_flagController.FlagPosition, OnNewBaseBuilt);
        _unitControll.RemoveUnit(builder);
    }

    private void OnNewBaseBuilt(Unit builder)
    {
        Base newBase = Instantiate(_basePrefab, builder.transform.position, Quaternion.identity);
        newBase.GetComponent<UnitControll>().AddUnit(builder);
        builder.SetBase(newBase);

        _flagController.RemoveFlag();
        StopBuilding();
    }
}