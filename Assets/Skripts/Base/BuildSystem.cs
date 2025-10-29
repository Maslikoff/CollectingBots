using UnityEngine;

public class BuildSystem : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Flag _flagPrefab;
    [SerializeField] private Base _basePrefab;

    [Header("Settings")]
    [SerializeField] private int _buildCost = 5;

    private Flag _currentFlag;
    private Vector3 _buildPosition;
    private Base _currentBase;

    private bool _isBuilding = false;

    public void TryCreateBuildSite(Vector3 position)
    {
        if (_isBuilding)
            return;

        Cleanup();
        _buildPosition = position;
        _currentFlag = Instantiate(_flagPrefab, position, Quaternion.identity);
    }

    public void OnBuildClicked()
    {
        if (_isBuilding)
            return;

        Base playerBase = FindPlayerBase();

        if (playerBase == null)
            return;

        if (CanBuild(playerBase) == false)
            return;

        Unit builder = FindBuilder(playerBase);

        if (builder == null)
            return;

        if (playerBase.SpendResources(_buildCost) == false)
            return;

        _currentBase = playerBase;
        _isBuilding = true;

        playerBase.StartBuilding();

        StartConstruction(builder, playerBase);
    }

    public void OnCancelClicked()
    {
        Cleanup();
    }

    private bool CanBuild(Base playerBase)
    {
        return playerBase.AvailableResources >= _buildCost && playerBase.AvailableUnitsCount > 1;
    }

    private void StartConstruction(Unit builder, Base playerBase)
    {
        builder.StartBuildMission(_buildPosition, () => { OnConstructionComplete(builder, playerBase); });
    }

    private void OnConstructionComplete(Unit builder, Base playerBase)
    {
        Base newBase = Instantiate(_basePrefab, _buildPosition, Quaternion.identity);

        TransferUnitToNewBase(builder, playerBase, newBase);
         _isBuilding = false;
        playerBase.FinishBuilding();
        Cleanup();
    }

    private void TransferUnitToNewBase(Unit unit, Base fromBase, Base toBase)
    {
        fromBase.RemoveUnit(unit);
        toBase.AddUnit(unit);
    }

    private Unit FindBuilder(Base playerBase)
    {
        var units = playerBase.AvailableUnits;

        foreach (Unit unit in units)
            if (unit.IsAvailable && IsUnitFromBase(unit, playerBase))
                return unit;

        return null;
    }

    private bool IsUnitFromBase(Unit unit, Base playerBase)
    {
        return playerBase.AvailableUnits.Contains(unit);
    }

    private Base FindPlayerBase()
    {
        return FindObjectOfType<Base>();
    }

    private void Cleanup()
    {
        if (_currentFlag != null)
        {
            Destroy(_currentFlag.gameObject);
            _currentFlag = null;
        }
    }
}