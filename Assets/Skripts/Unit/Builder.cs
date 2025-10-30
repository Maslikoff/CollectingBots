using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    [SerializeField] private float _buildTime = 2f;

    private Coroutine _buildCoroutine;

    public bool IsBuilding => _buildCoroutine != null;

    public event Action<Builder> BuildStarted;
    public event Action<Builder> BuildCompleted;

    private void OnDestroy()
    {
        if (_buildCoroutine != null)
            StopCoroutine(_buildCoroutine);
    }

    public void StartBuilding(Action onBuildComplete = null)
    {
        if (IsBuilding)
            return;

        _buildCoroutine = StartCoroutine(BuildProcess(onBuildComplete));
        BuildStarted?.Invoke(this);
    }

    private IEnumerator BuildProcess(Action onBuildComplete)
    {
        yield return new WaitForSeconds(_buildTime);

        onBuildComplete?.Invoke();
        BuildCompleted?.Invoke(this);
        _buildCoroutine = null;
    }

    public void StopBuilding()
    {
        if (_buildCoroutine != null)
        {
            StopCoroutine(_buildCoroutine);
            _buildCoroutine = null;
        }
    }
}
