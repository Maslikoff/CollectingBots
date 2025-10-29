using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBuilder : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Flag _flagPrefab;
    [SerializeField] private Base _basePrefab;
    [SerializeField] private BuildUI _buildUIPrefab;

    [Header("Settings")]
    [SerializeField] private int _buildCost = 5;

    private Flag _currentFlag;
    private BuildUI _currentBuildUI;
    private Base _parentBase;
    private Unit _currentBuilder;
    private Vector3 _buildPosition;

    private bool _isBuilding = false;

    public bool IsBuilding => _isBuilding;
    public bool HasActiveFlag => _currentFlag != null;

    public void Initialize(Base parentBase)
    {
        _parentBase = parentBase;
        Debug.Log($"BaseBuilder initialized for base: {_parentBase.name}");
    }

    public void TryCreateBuildSite(Vector3 position)
    {
        if (_isBuilding)
            return;

        Cleanup();

        _buildPosition = position;
        _currentFlag = Instantiate(_flagPrefab, _buildPosition, Quaternion.identity);
        _currentBuildUI = Instantiate(_buildUIPrefab);
        _currentBuildUI.Initialize(this, position);

        Debug.Log($"Build site created at: {position}");
    }

    public void StartConstruction()
    {
        Debug.Log("=== START CONSTRUCTION ===");

        if (_isBuilding)
        {
            Debug.Log("Already building!");
            return;
        }

        if (_currentFlag == null)
        {
            Debug.Log("No flag!");
            return;
        }

        if (_parentBase == null)
        {
            Debug.LogError("Parent base is null!");
            return;
        }

        if (CanBuild() == false)
        {
            Debug.Log("Cannot build: conditions not met");
            return;
        }

        _currentBuilder = FindBuilder();

        if (_currentBuilder == null)
        {
            Debug.Log("No available builder found!");
            return;
        }

        if (_parentBase.SpendResourcesForBuilding(_buildCost) == false)
        {
            Debug.Log("Failed to spend resources for building");
            return;
        }

        _isBuilding = true;

        if (_currentBuildUI != null)
            _currentBuildUI.Hide();

        Debug.Log($"Starting construction with builder: {_currentBuilder.name}");
        _currentBuilder.StartBuildMission(_buildPosition, OnConstructionComplete);
    }

    public void CancelConstruction()
    {
        if (_currentBuilder != null)
            _currentBuilder.GetComponent<Builder>().CancelBuild();

        Cleanup();
    }

    private bool CanBuild()
    {
        if (_parentBase == null)
            return false;

        bool hasResources = _parentBase.TotalResources >= _buildCost;
        bool hasUnits = _parentBase.AvailableUnitsCount > 1;

        Debug.Log($"CanBuild - Total Resources: {_parentBase.TotalResources}/{_buildCost}, Available Resources: {_parentBase.AvailableResources}/{_buildCost}, Units: {_parentBase.AvailableUnitsCount}");
        Debug.Log($"CanBuild - Result: {hasResources && hasUnits}");

        return hasResources && hasUnits;
    }


    private Unit FindBuilder()
    {
        if (_parentBase == null || _parentBase.AvailableUnits == null)
        {
            Debug.LogError("Parent base or available units is null!");
            return null;
        }

        Debug.Log($"Looking for builder. Available units: {_parentBase.AvailableUnits.Count}");

        foreach (Unit unit in _parentBase.AvailableUnits)
        {
            Debug.Log($"Checking unit: {unit.name}, IsAvailable: {unit.IsAvailable}, IsBuilder: {unit.IsBuilder}");

            if (unit.IsAvailable && unit.IsBuilder)
            {
                Debug.Log($"Found available builder: {unit.name}");
                return unit;
            }
        }

        Debug.Log("No available builder found in all units");
        return null;
    }

    private void OnConstructionComplete()
    {
        Debug.Log("=== CONSTRUCTION COMPLETE ===");

        Base newBase = Instantiate(_basePrefab, _buildPosition, Quaternion.identity);
        Debug.Log($"New base created: {newBase.name}");

        if (_currentBuilder != null)
        {
            Debug.Log($"Transferring builder {_currentBuilder.name} to new base");
            TransferUnitToNewBase(_currentBuilder, _parentBase, newBase);
            _currentBuilder = null;
        }

        _isBuilding = false;
        Cleanup();
    }

    private void TransferUnitToNewBase(Unit unit, Base fromBase, Base toBase)
    {
        fromBase.RemoveUnit(unit);
        toBase.AddUnit(unit);
        Debug.Log($"Unit {unit.name} transferred from {fromBase.name} to {toBase.name}");
    }

    private void Cleanup()
    {
        if (_currentFlag != null)
        {
            Destroy(_currentFlag.gameObject);
            _currentFlag = null;
        }

        if (_currentBuildUI != null)
        {
            Destroy(_currentBuildUI.gameObject);
            _currentBuildUI = null;
        }

        _isBuilding = false;
        _currentBuilder = null;
    }
}
