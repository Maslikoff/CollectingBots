using UnityEngine;

public class UnitPool : ObjectPool<Unit>
{
    [SerializeField] private Transform _spawnPoint;

    private Base _ownerBase;

    protected override void OnObjectGet(Unit unit)
    {
        base.OnObjectGet(unit);

        unit.Initialize(_ownerBase);

        if (_spawnPoint != null)
        {
            Vector3 spawnPosition = _spawnPoint.position + Random.insideUnitSphere * 2f;
            spawnPosition.y = 1f; 
            unit.transform.position = spawnPosition;
        }
    }

    protected override void ResetObject(Unit unit)
    {
        base.ResetObject(unit);

        Vector3 resetPosition = unit.transform.position;
        resetPosition.y = 1f;
        unit.transform.position = resetPosition;
    }

    public void Initialize(Base ownerBase)
    {
        _ownerBase = ownerBase;
    }

    public Unit GetAvailableUnit() => GetObject();
}