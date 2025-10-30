using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Base : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _resourcesForNewUnit = 3;
    [SerializeField] private int _resourcesForNewBase = 5;
    [SerializeField] private int _minUnitsForNewBase = 2;

    [Header("Dependencies")]
    [SerializeField] private UnitPool _unitPool;
    [SerializeField] private ResourcePool _resourcePool;
    [SerializeField] private Flag _flagPrefab;
    [SerializeField] private Base _basePrefab;
    [SerializeField] private MapBounds _mapBounds;

    private List<Unit> _units = new List<Unit>();
    private List<Resource> _collectedResources = new List<Resource>();
    private Flag _currentFlag;
    private bool _isBuildingNewBase = false;
    private int _totalResources = 0;

    public int TotalResources => _totalResources;
    public int UnitsCount => _units.Count;
    public bool HasFlag => _currentFlag != null;

    private void Start()
    {
        CreateInitialUnit();
    }

    private void CreateInitialUnit()
    {
        CreateNewUnit();
    }

    private void Update()
    {
        if (!_isBuildingNewBase)
        {
            // Механика 1: Создание нового юнита за 3 ресурса
            if (_totalResources >= _resourcesForNewUnit)
            {
                _totalResources -= _resourcesForNewUnit;
                CreateNewUnit();
            }
        }
        else
        {
            // Механика 4: Строительство новой базы за 5 ресурсов
            if (_totalResources >= _resourcesForNewBase && _units.Count > _minUnitsForNewBase)
            {
                _totalResources -= _resourcesForNewBase;
                SendUnitToBuildBase();
            }
        }
    }

    public void AddResource(Resource resource)
    {
        _collectedResources.Add(resource);
        _totalResources++;

        if (_resourcePool != null)
            _resourcePool.ReturnResource(resource);
    }

    private void CreateNewUnit()
    {
        Unit newUnit = _unitPool.GetObject();
        newUnit.Initialize(this);
        _units.Add(newUnit);
    }

    private void SendUnitToBuildBase()
    {
        if (_units.Count == 0 || _currentFlag == null) return;

        // Находим свободного юнита-строителя
        Unit builder = _units.Find(unit => unit.IsAvailable && unit.IsBuilder);
        if (builder == null) return;

        // Механика 4: Отправляем юнита строить новую базу
        builder.BuildNewBase(_currentFlag.transform.position, OnNewBaseBuilt);
        _units.Remove(builder);
    }

    private void OnNewBaseBuilt(Unit builder)
    {
        // Механика 5: Создаем новую базу и убираем флаг
        Base newBase = Instantiate(_basePrefab, builder.transform.position, Quaternion.identity);
        newBase.CreateInitialUnit();

        // Передаем строителя новой базе
        newBase.AddUnit(builder);
        builder.SetBase(newBase);

        RemoveFlag();
        _isBuildingNewBase = false;
    }

    public void PlaceFlag(Vector3 position)
    {
        // Механика 3: Установка/перемещение флага
        if (!_mapBounds.IsPositionInsideMap(position)) return;

        if (_currentFlag == null)
        {
            _currentFlag = Instantiate(_flagPrefab, position, Quaternion.identity);
        }
        else
        {
            _currentFlag.transform.position = position;
        }

        _isBuildingNewBase = true;
    }

    public void RemoveFlag()
    {
        if (_currentFlag != null)
        {
            Destroy(_currentFlag.gameObject);
            _currentFlag = null;
        }
    }

    public void AddUnit(Unit unit)
    {
        _units.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        _units.Remove(unit);
    }
}