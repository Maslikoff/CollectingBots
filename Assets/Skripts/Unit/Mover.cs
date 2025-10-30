using System;
using UnityEngine;

public class Mover : MonoBehaviour
{
    private const float MinDistance = 1f;

    [SerializeField] private float _speed = 5f;

    private Vector3 _targetPosition;
    private bool _isMoving = false;

    private Action ReachedTarget;

    public bool IsMoving => _isMoving;
    public Vector3 TargetPosition => _targetPosition;

    private void Update()
    {
        if (_isMoving)
            MoveToTarget();
    }

    public void MoveTo(Vector3 targetPosition, Action OnReachedTarget = null)
    {
        _targetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        _isMoving = true;
        ReachedTarget = OnReachedTarget;
    }

    private void MoveToTarget()
    {
        Vector3 direction = (_targetPosition - transform.position).normalized;
        transform.position += direction * _speed * Time.deltaTime;

        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);

        float distance = Vector3.Distance(transform.position, _targetPosition);

        if (distance < MinDistance)
        {
            _isMoving = false;
            ReachedTarget?.Invoke();
            ReachedTarget = null;
        }
    }
}