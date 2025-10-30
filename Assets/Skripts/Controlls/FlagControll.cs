using System;
using UnityEngine;

public class FlagControll : MonoBehaviour
{
    [SerializeField] private Flag _flagPrefab;
    [SerializeField] private MapBounds _mapBounds;

    private Flag _currentFlag;

    public bool HasFlag => _currentFlag != null;
    public Vector3 FlagPosition => _currentFlag != null ? _currentFlag.transform.position : Vector3.zero;

    public event Action<Vector3> FlagPlaced;
    public event Action FlagRemoved;

    public bool PlaceFlag(Vector3 position)
    {
        if (_mapBounds.IsPositionInsideMap(position) == false)
            return false;

        if (_currentFlag == null)
            _currentFlag = Instantiate(_flagPrefab, position, Quaternion.identity);
        else
            _currentFlag.transform.position = position;

        FlagPlaced?.Invoke(position);

        return true;
    }

    public void RemoveFlag()
    {
        if (_currentFlag != null)
        {
            Destroy(_currentFlag.gameObject);
            _currentFlag = null;
            FlagRemoved?.Invoke();
        }
    }
}