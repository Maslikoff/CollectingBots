using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitControll : MonoBehaviour
{
    [SerializeField] private UnitPool _unitPool;
    [SerializeField] private int _initialUnits = 3;

    private List<Unit> _units = new List<Unit>();
    private ResourceHub _resourceHub;

    public int UnitsCount => _units.Count;
    public IReadOnlyList<Unit> Units => _units;

    public event Action<Unit> UnitAdded;
    public event Action<Unit> UnitRemoved;

    public void Initialize(ResourceHub resourceHub)
    {
        _resourceHub = resourceHub;

        CreateInitialUnits();
    }

    private void CreateInitialUnits()
    {
        for (int i = 0; i < _initialUnits; i++)
            CreateNewUnit();
    }

    public void CreateNewUnit()
    {
        if (_unitPool == null)
            return;

        Unit newUnit = _unitPool.GetAvailableUnit();
        AddUnit(newUnit);
    }

    public void AddUnit(Unit unit)
    {
        if(_units.Contains(unit)) 
            return;

        _units.Add(unit);
        SetupUnitEvents(unit);
        UnitAdded?.Invoke(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        if (_units.Remove(unit))
            UnitRemoved?.Invoke(unit);
    }

    public Unit GetAvailableUnit()
    {
        foreach (Unit unit in _units)
            if (unit.IsAvailable)
                return unit;
 
        return null;
    }

    public List<Unit> GetAvailableUnits()
    {
        List<Unit> availableUnits = new List<Unit>();

        foreach (Unit unit in _units)
            if (unit.IsAvailable)
                availableUnits.Add(unit);

        return availableUnits;
    }

    private void SetupUnitEvents(Unit unit)
    {
        unit.BecameAvailable -= OnUnitBecameAvailable;
        unit.BecameBusy -= OnUnitBecameBusy;
        unit.ResourceDelivered -= OnResourceDelivered;

        unit.BecameAvailable += OnUnitBecameAvailable;
        unit.BecameBusy += OnUnitBecameBusy;
        unit.ResourceDelivered += OnResourceDelivered;
    }

    private void OnUnitBecameAvailable(Unit unit)
    {
        if (_units.Contains(unit) == false)
            AddUnit(unit);
    }

    private void OnUnitBecameBusy(Unit unit)
    {
        RemoveUnit(unit);
    }

    private void OnResourceDelivered(Unit unit, ITakeResource resource)
    {
        if (resource == null && _resourceHub == null)
            return;

        if (resource is Resource resourceObj)
            _resourceHub.AddResource(resourceObj);
    }
}