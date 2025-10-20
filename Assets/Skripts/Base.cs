using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private UnitPool _unitPool;
    [SerializeField] private int _initialUnits = 3;
    [SerializeField] private float _scanInterval = .1f;

    private List<Unit> _availableUnits;
    private ResourcePool _resourcePool;
    private int _totalResources = 0;
    private float _scanTimer;

    public int TotalResources => _totalResources;

    private void Start()
    {
        _availableUnits = new List<Unit>();
        _resourcePool = FindObjectOfType<ResourcePool>();

        if (_unitPool != null)
        {
            _unitPool.Initialize(this);

            for (int i = 0; i < _initialUnits; i++)
            {
                Unit unit = _unitPool.GetAvailableUnit();
                _availableUnits.Add(unit);
            }
        }

        _scanTimer = _scanInterval;
    }

    private void Update()
    {
        _scanTimer -= Time.deltaTime;

        if (_scanTimer <= 0)
        {
            ScanForResources();
            _scanTimer = _scanInterval;
        }
    }

    private void ScanForResources()
    {
        if (_availableUnits.Count == 0 || _resourcePool == null) 
            return;

        List<ITakeResource> availableResources = _resourcePool.GetAvailableResources();

        foreach (ITakeResource resource in availableResources)
        {
            if (_availableUnits.Count > 0 && !resource.IsCollected)
            {
                Unit unit = _availableUnits[0];
                _availableUnits.RemoveAt(0);

                unit.AssignResource(resource, OnUnitReachedResource);
            }
        }
    }

    private void OnUnitReachedResource(Unit unit, ITakeResource resource)
    {
        unit.SendToBase(transform.position, OnUnitReachedBase);
    }

    private void OnUnitReachedBase(Unit unit)
    {
        AddResource(1);

        if (_availableUnits.Contains(unit) == false)
            _availableUnits.Add(unit);
    }

    public void AddResource(int amount)
    {
        _totalResources += amount;
        Debug.Log($"Resources collected: {_totalResources}");
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
}