using System;
using UnityEngine;

[RequireComponent(typeof(UnitControll))]
[RequireComponent(typeof(FlagControll))]
[RequireComponent(typeof(BaseBuilder))]
[RequireComponent(typeof(UnitFactory))]
[RequireComponent(typeof(UnitAssigner))]
public class Base : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private ResourceHub _resourceHub;

    private UnitControll _unitControll;
    private FlagControll _flagControll;
    private BaseBuilder _baseBuilder;
    private UnitFactory _unitFactory;
    private UnitAssigner _resourceCollector;

    private bool _shouldCreateInitialUnits = true;
    private bool _isInitialized = false;

    public int TotalResources => _resourceHub?.TotalResources ?? 0;
    public int UnitsCount => _unitControll?.UnitsCount ?? 0;
    public bool HasFlag => _flagControll?.HasFlag ?? false;
    public bool IsBuildingNewBase => _baseBuilder?.IsBuilding ?? false;

    public event Action<int> ResourcesChanged;
    public event Action<bool> BuildingModeChanged;

    private void Awake()
    {
        _unitControll = GetComponent<UnitControll>();
        _flagControll = GetComponent<FlagControll>();
        _baseBuilder = GetComponent<BaseBuilder>();
        _unitFactory = GetComponent<UnitFactory>();
        _resourceCollector = GetComponent<UnitAssigner>();
    }

    private void Start()
    {
        if (_shouldCreateInitialUnits && _isInitialized == false)
            InitializeBase(true);
    }

    private void Update()
    {
        if (_isInitialized == false) 
            return;

        if (IsBuildingNewBase == false)
            _unitFactory.UpdateProduction();
        else
            _baseBuilder.UpdateBuilding();
    }

    public void InitializeBase(bool createInitialUnits)
    {
        if (_isInitialized)
            return;

        _shouldCreateInitialUnits = createInitialUnits;

        _unitControll.Initialize(_resourceHub, this, createInitialUnits);
        _baseBuilder.Initialize(_unitControll, _resourceHub, _flagControll, this);
        _unitFactory.Initialize(_unitControll, _resourceHub, this);
        _resourceCollector.Initialize(_unitControll, _resourceHub);

        _resourceHub.ResourcesChanged += OnResourcesChanged;
        _flagControll.FlagPlaced += OnFlagPlaced;
        _baseBuilder.BuildingModeChanged += OnBuildingModeChanged;

        ResourcesChanged?.Invoke(TotalResources);

        _isInitialized = true;
    }

    public void SetAsNewBase()
    {
        Vector3 position = transform.position;
        position.y = 0f;
        transform.position = position;

        InitializeBase(false);
    }

    public void PlaceFlag(Vector3 position)
    {
        _flagControll.PlaceFlag(position);
    }

    public void AddUnit(Unit unit)
    {
        if (unit == null && _unitControll == null)
            return;

        _unitControll.AddUnit(unit);
    }

    private void OnResourcesChanged(int resources) 
    {
        ResourcesChanged?.Invoke(resources); 
    }
    private void OnFlagPlaced(Vector3 position) 
    {
        _baseBuilder.StartBuilding();
    }
    private void OnBuildingModeChanged(bool isBuilding)
    {
        BuildingModeChanged?.Invoke(isBuilding);
    }

    private void OnDestroy()
    {
        _resourceHub.ResourcesChanged -= OnResourcesChanged;
        _flagControll.FlagPlaced -= OnFlagPlaced;
        _baseBuilder.BuildingModeChanged -= OnBuildingModeChanged;
    }
}