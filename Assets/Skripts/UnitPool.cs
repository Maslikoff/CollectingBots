using System.Collections;
using System.Collections.Generic;
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
            unit.transform.position = _spawnPoint.position + Random.insideUnitSphere * 2f;
    }

    protected override void ResetObject(Unit unit)
    {
        base.ResetObject(unit);
    }

    public void Initialize(Base ownerBase)
    {
        _ownerBase = ownerBase;
    }

    public Unit GetAvailableUnit() => GetObject();
}