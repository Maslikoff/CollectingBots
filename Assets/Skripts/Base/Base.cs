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
            // �������� 1: �������� ������ ����� �� 3 �������
            if (_totalResources >= _resourcesForNewUnit)
            {
                _totalResources -= _resourcesForNewUnit;
                CreateNewUnit();
            }
        }
        else
        {
            // �������� 4: ������������� ����� ���� �� 5 ��������
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

        // ������� ���������� �����-���������
        Unit builder = _units.Find(unit => unit.IsAvailable && unit.IsBuilder);
        if (builder == null) return;

        // �������� 4: ���������� ����� ������� ����� ����
        builder.BuildNewBase(_currentFlag.transform.position, OnNewBaseBuilt);
        _units.Remove(builder);
    }

    private void OnNewBaseBuilt(Unit builder)
    {
        // �������� 5: ������� ����� ���� � ������� ����
        Base newBase = Instantiate(_basePrefab, builder.transform.position, Quaternion.identity);
        newBase.CreateInitialUnit();

        // �������� ��������� ����� ����
        newBase.AddUnit(builder);
        builder.SetBase(newBase);

        RemoveFlag();
        _isBuildingNewBase = false;
    }

    public void PlaceFlag(Vector3 position)
    {
        // �������� 3: ���������/����������� �����
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