using System.Collections.Generic;
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

    public void ClearPool()
    {
        ResetPool();

        var poolField = typeof(ObjectPool<Unit>).GetField("_pool", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (poolField != null)
        {
            var pool = (Queue<Unit>)poolField.GetValue(this);
            pool.Clear();
        }
    }

    public List<Unit> GetAllActiveUnits()
    {
        var activeUnits = new List<Unit>();

        var activeObjectsField = typeof(ObjectPool<Unit>).GetField("_activeObjects", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (activeObjectsField != null)
        {
            var activeObjects = (HashSet<Unit>)activeObjectsField.GetValue(this);
            activeUnits.AddRange(activeObjects);
        }

        return activeUnits;
    }
}