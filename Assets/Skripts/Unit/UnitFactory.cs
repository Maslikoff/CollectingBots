using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitFactory : MonoBehaviour
{
    [SerializeField] private UnitPool _unitPool;
    [SerializeField] private int _unitCost = 3;

    private List<Unit> _createdUnits = new List<Unit>();

    public event Action<Unit> UnitCreated
    {
        add => _unitCreated += value;
        remove => _unitCreated -= value;
    }

    private Action<Unit> _unitCreated;

    public int UnitCost => _unitCost;
    public int CreatedUnitCount => _createdUnits.Count;
    public int TotalCostSpent => CreatedUnitCount * _unitCost;

    public bool CanCreateUnit(int availableResources) => availableResources >= _unitCost;

    public Unit CreateUnit(Vector3 spawnPosition)
    {
        Unit newUnit = _unitPool.GetAvailableUnit();
        newUnit.transform.position = spawnPosition;
        _createdUnits.Add(newUnit);
        _unitCreated?.Invoke(newUnit);

        return newUnit;
    }

    public void SetupUnitEvents(Unit unit, Action<Unit> onBecameAvailable, Action<Unit> onBecameBusy, Action<Unit, ITakeResource> onResourceDelivered)
    {
        unit.BecameAvailable += onBecameAvailable;
        unit.BecameBusy += onBecameBusy;
        unit.ResourceDelivered += onResourceDelivered;
    }

    public void Cleanup()
    {
        _createdUnits.Clear();
    }
}