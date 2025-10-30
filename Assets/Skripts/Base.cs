using System;
using UnityEngine;

[RequireComponent(typeof(UnitControll))]
[RequireComponent(typeof(FlagControll))]
[RequireComponent(typeof(BaseBuilder))]
[RequireComponent(typeof(UnitFactory))]
[RequireComponent(typeof(ResourceCollector))]
public class Base : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private ResourceHub _resourceHub;

    private UnitControll _unitControll;
    private FlagControll _flagControll;
    private BaseBuilder _baseBuilder;
    private UnitFactory _unitFactory;
    private ResourceCollector _resourceCollector;

    public int TotalResources => _resourceHub.TotalResources;
    public int UnitsCount => _unitControll.UnitsCount;
    public bool HasFlag => _flagControll.HasFlag;
    public bool IsBuildingNewBase => _baseBuilder.IsBuilding;

    public event Action<int> ResourcesChanged;
    public event Action<bool> BuildingModeChanged;

    private void Awake()
    {
        _unitControll = GetComponent<UnitControll>();
        _flagControll = GetComponent<FlagControll>();
        _baseBuilder = GetComponent<BaseBuilder>();
        _unitFactory = GetComponent<UnitFactory>();
        _resourceCollector = GetComponent<ResourceCollector>();
    }

    private void Start()
    {
        InitializeComponents();

        ResourcesChanged?.Invoke(TotalResources);
    }

    private void Update()
    {
        if (IsBuildingNewBase == false)
            _unitFactory.UpdateProduction();
        else
            _baseBuilder.UpdateBuilding();
    }

    public void PlaceFlag(Vector3 position)
    {
        _flagControll.PlaceFlag(position);
    }

    public void AddUnit(Unit unit)
    {
        _unitControll.AddUnit(unit);
    }

    private void InitializeComponents()
    {
       _unitControll.Initialize(_resourceHub);
       _baseBuilder.Initialize(_unitControll, _resourceHub, _flagControll);
       _unitFactory.Initialize(_unitControll, _resourceHub);
       _resourceCollector.Initialize(_unitControll, _resourceHub);

        _resourceHub.ResourcesChanged += OnResourcesChanged;
        _flagControll.FlagPlaced += OnFlagPlaced;
        _baseBuilder.BuildingModeChanged += OnBuildingModeChanged;
    }

    private void OnResourcesChanged(int resources) => ResourcesChanged?.Invoke(resources);
    private void OnFlagPlaced(Vector3 position) => _baseBuilder.StartBuilding();
    private void OnBuildingModeChanged(bool isBuilding) => BuildingModeChanged?.Invoke(isBuilding);

    private void OnDestroy()
    {
        _resourceHub.ResourcesChanged -= OnResourcesChanged;
        _flagControll.FlagPlaced -= OnFlagPlaced;
        _baseBuilder.BuildingModeChanged -= OnBuildingModeChanged;
    }
}