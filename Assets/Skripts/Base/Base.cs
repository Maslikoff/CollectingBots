using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Base : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private UnitFactory _unitFactory;
    [SerializeField] private ResourceCollector _resourceCollector;
    [SerializeField] private ResourceHub _resourceHub;
    [SerializeField] private BaseFlagControll _flagControll;

    [Header("Settings")]
    [SerializeField] private ResourceType _allowedResources = ResourceType.Nothing;
    [SerializeField] private int _initialUnits = 3;
    [SerializeField] private float _scanInterval = 1f;

    private List<Unit> _availableUnits;
    private Transform _transform;
    private Coroutine _scanCoroutine;

    private bool _isBuildingInProgress = false;

    public int TotalResources => _resourceCollector.TotalResources;
    public int AvailableResources => _resourceCollector.AvailableResources;
    public int AvailableUnitsCount => _availableUnits.Count;
    public ResourceType AllowedResources => _allowedResources;
    public bool HasFlag => _flagControll.HasFlag;
    public Vector3 FlagPosition => _flagControll.FlagPosition;
    public List<Unit> AvailableUnits => _availableUnits;

    public event Action<ResourceType> ResourceTypeChanged;
    public event Action<int> ResourcesChanged;
    public event Action<Vector3> BasePositionChanged;

    private void Start()
    {
        _availableUnits = new List<Unit>();
        _transform = transform;

        InitializeComponents();
        CreateInitialUnits();

        _scanCoroutine = StartCoroutine(ScanForResourcesRoutine());
    }

    private void OnDestroy()
    {
        if (_scanCoroutine != null)
            StopCoroutine(_scanCoroutine);

        _unitFactory.Cleanup();
    }

    public void SetAllowedResource(ResourceType newResourceType)
    {
        if (_allowedResources != newResourceType)
        {
            _allowedResources = newResourceType;
            ResourceTypeChanged?.Invoke(_allowedResources);
            UpdateUnitsResourceType();
        }
    }

    public void OnBaseMoved(Vector3 newPosition)
    {
        BasePositionChanged?.Invoke(newPosition);

        ReturnAllUnitsToBase(newPosition);
        RedirectBusyUnits(newPosition);
    }

    public bool SpendResources(int amount)
    {
        return _resourceCollector.TrySpendResources(amount);
    }

    public void AddResources(int amount)
    {
        for (int i = 0; i < amount; i++)
            _resourceCollector.AddResource();
    }

    public void RemoveUnit(Unit unit)
    {
        AvailableUnits.Remove(unit);
    }

    public void AddUnit(Unit unit)
    {
        AvailableUnits.Add(unit);
    }

    public void StartBuilding()
    {
        _isBuildingInProgress = true;
    }

    public void FinishBuilding()
    {
        _isBuildingInProgress = false;
    }

    public bool IsBuildingInProgress()
    {
        return _isBuildingInProgress;
    }

    private void ReturnAllUnitsToBase(Vector3 basePosition)
    {
        foreach (Unit unit in _availableUnits)
            if (unit.IsAvailable && unit.IsBusy == false)
                unit.MoveToPosition(basePosition);
    }

    private void RedirectBusyUnits(Vector3 basePosition)
    {
        foreach (Unit unit in _availableUnits)
            if (unit.IsBusy)
                unit.ReturnToBase(basePosition);
    }

    private void UpdateUnitsResourceType()
    {
        foreach (Unit unit in _availableUnits)
            if (unit is IResourceTypeListener resourceListener)
                resourceListener.OnResourceTypeChanged(_allowedResources);
    }

    private void InitializeComponents()
    {
        _unitFactory.UnitCreated += OnUnitCreated;
        _resourceCollector.ResourcesChanged += OnResourcesChanged;
        _resourceCollector.ResourcesChanged += (resources) => ResourcesChanged?.Invoke(resources);
        _flagControll.FlagPlaced += OnFlagPlaced;
    }

    private void CreateInitialUnits()
    {
        for (int i = 0; i < _initialUnits; i++)
            CreateNewUnit();
    }

    private IEnumerator ScanForResourcesRoutine()
    {
        var wait = new WaitForSeconds(_scanInterval);

        while (enabled)
        {
            yield return wait;
            AssignUnitsToResources();
            TryCreateNewUnit();
        }
    }

    private void AssignUnitsToResources()
    {
        if (_availableUnits.Count == 0 || _resourceHub == null)
            return;

        if (_allowedResources == ResourceType.Nothing)
            return;

        if (IsBuildingInProgress()) 
            return;

        foreach (Unit unit in _availableUnits.Where(unit => unit.IsAvailable).ToList())
        {
            ITakeResource resource = _resourceHub.GetAvailableResource(_allowedResources);

            if (resource != null)
            {
                unit.AssignToCollectResource(resource, transform.position);
                _availableUnits.Remove(unit);
            }
            else
            {
                break;
            }
        }
    }

    private void CreateNewUnit()
    {
        Vector3 spawnPosition = GetSpawnPosition();
        Unit unit = _unitFactory.CreateUnit(spawnPosition);

        if (unit is IResourceTypeListener resourceListener)
            resourceListener.OnResourceTypeChanged(_allowedResources);

        BasePositionChanged += unit.ReturnToBase;

        _unitFactory.SetupUnitEvents(unit, OnUnitBecameAvailable, OnUnitBecameBusy, OnResourceDelivered);
        _availableUnits.Add(unit);
    }

    private void TryCreateNewUnit()
    {
        if (_unitFactory.CanCreateUnit(AvailableResources))
            if (_resourceCollector.TrySpendResources(_unitFactory.UnitCost))
                CreateNewUnit();
    }

    private Vector3 GetSpawnPosition() => transform.position + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));

    private void OnResourcesChanged(int availableResources)
    {
        ResourcesChanged?.Invoke(availableResources);
        TryCreateNewUnit();
    }

    private void OnUnitCreated(Unit unit)
    {
        if (unit is IResourceTypeListener resourceListener)
            resourceListener.OnResourceTypeChanged(_allowedResources);
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

    private void OnResourceDelivered(Unit unit, ITakeResource resource)
    {
        _resourceHub.MarkResourceAsDelivered(resource);
        _resourceCollector.AddResource();
        TryCreateNewUnit();
    }

    private void OnFlagPlaced(Vector3 position) 
    {
        OnBaseMoved(position);
    }
}