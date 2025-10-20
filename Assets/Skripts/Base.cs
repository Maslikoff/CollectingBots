using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private UnitPool _unitPool;
    [SerializeField] private ResourceFactory _resourceFactory;
    [SerializeField] private int _initialUnits = 3;
    [SerializeField] private float _scanInterval = 3f;

    private List<Unit> _availableUnits;
    private int _totalResources = 0;
    private Coroutine _scanCoroutine;

    public int TotalResources => _totalResources;

    public event Action<int> ResourcesChanged;

    private void Start()
    {
        _availableUnits = new List<Unit>();

        if (_unitPool != null)
        {
            _unitPool.Initialize(this);

            for (int i = 0; i < _initialUnits; i++)
            {
                Unit unit = _unitPool.GetAvailableUnit();
                _availableUnits.Add(unit);
            }
        }

        ResourcesChanged?.Invoke(_totalResources);

        _scanCoroutine = StartCoroutine(ScanForResourcesRoutine());
    }

    private void OnDestroy()
    {
        if (_scanCoroutine != null)
            StopCoroutine(_scanCoroutine);
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

    public void AddResource()
    {
        _totalResources++;

        ResourcesChanged?.Invoke(_totalResources);
    }

    public void UnitBecameAvailable(Unit unit)
    {
        if (_availableUnits.Contains(unit) == false)
            _availableUnits.Add(unit);
    }

    public void UnitBecameBusy(Unit unit)
    {
        _availableUnits.Remove(unit);
    }

    private void AssignUnitsToResources()
    {
        if (_availableUnits.Count == 0 || _resourceFactory == null)
            return;

        foreach (Unit unit in _availableUnits.ToArray())
        {
            if (unit.IsAvailable)
            {
                ITakeResource resource = _resourceFactory.GetAvailableResource();

                if (resource != null)
                {
                    unit.AssignToCollectResource(resource, OnResourcePickedUp, OnResourceDelivered);
                    _availableUnits.Remove(unit);
                }
            }
        }
    }

    private void OnResourcePickedUp(Unit unit)
    {
        Debug.Log("Unit picked up resource");
    }

    private void OnResourceDelivered(Unit unit)
    {
        if (unit.CarriedResource != null)
            _resourceFactory.MarkResourceAsDelivered(unit.CarriedResource);

        AddResource();

        if (_availableUnits.Contains(unit) == false)
            _availableUnits.Add(unit);
    }
}