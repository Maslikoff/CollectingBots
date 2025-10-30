using System;
using UnityEngine;

public class BaseFlagControll : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Flag _flagPrefab;
    [SerializeField] private Base _base;

    [Header("Settings")]
    [SerializeField] private Color _flagColor = Color.blue;

    private Flag _currentFlag;
    private bool _isPlacingFlag = false;

    public bool HasFlag => _currentFlag != null;
    public Vector3 FlagPosition => _currentFlag != null ? _currentFlag.transform.position : transform.position;

    public event Action<Vector3> FlagPlaced;

    public void TryPlaceFlag(Vector3 worldPosition)
    {
        Vector3 targetPosition = worldPosition;
        targetPosition.y = transform.position.y; 

        CreateTemporaryFlag(targetPosition);
        TeleportBase(targetPosition);
        RemoveFlag();

        FlagPlaced?.Invoke(targetPosition);
    }

    private void CreateTemporaryFlag(Vector3 position)
    {
        Flag tempFlag = Instantiate(_flagPrefab, position, Quaternion.identity);
        tempFlag.SetColor(_flagColor);
        tempFlag.ShowPlacementEffect();

        Destroy(tempFlag.gameObject, 2f);
    }

    private void TeleportBase(Vector3 targetPosition)
    {
        Vector3 oldPosition = transform.position;

        transform.position = targetPosition;
    }

    public void StartFlagPlacement()
    {
        _isPlacingFlag = true;
    }

    public void RemoveFlag()
    {
        if (_currentFlag != null)
        {
            Destroy(_currentFlag.gameObject);
            _currentFlag = null;
        }
    }

    private void OnBaseSelected(Base selectedBase)
    {
        _isPlacingFlag = (selectedBase == _base);
    }

    private void OnGroundClicked(Vector3 groundPosition)
    {
        if (_isPlacingFlag)
        {
            TryPlaceFlag(groundPosition);
            _isPlacingFlag = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (_isPlacingFlag)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 2f, 1.5f);
            Gizmos.DrawIcon(transform.position + Vector3.up * 3f, "d_Record@2x");
        }

        if (_currentFlag != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, _currentFlag.transform.position);
            Gizmos.DrawWireCube(_currentFlag.transform.position, Vector3.one * 0.5f);
        }
    }
}