using UnityEngine;

public class UnitPool : ObjectPool<Unit>
{
    [SerializeField] private Transform _spawnPoint;

    protected override void OnObjectGet(Unit unit)
    {
        base.OnObjectGet(unit);

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

    public Unit GetAvailableUnit() => GetObject();
}