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
            for (int i = 0; i < _initialUnits; i++)
            {
                Unit unit = _unitPool.GetAvailableUnit();
                SetupUnitEvents(unit);
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

    private void SetupUnitEvents(Unit unit)
    {
        unit.BecameAvailable += OnUnitBecameAvailable;
        unit.BecameBusy += OnUnitBecameBusy;
        unit.ResourceDelivered += OnResourceDelivered;
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

    public void OnUnitBecameAvailable(Unit unit)
    {
        if (_availableUnits.Contains(unit) == false)
            _availableUnits.Add(unit);
    }

    public void OnUnitBecameBusy(Unit unit)
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
                    unit.AssignToCollectResource(resource, transform.position);
                    _availableUnits.Remove(unit);
                }
            }
        }
    }

    private void OnResourceDelivered(Unit unit, ITakeResource resource)
    {
        _resourceFactory.MarkResourceAsDelivered(resource);
        AddResource();
    }
}