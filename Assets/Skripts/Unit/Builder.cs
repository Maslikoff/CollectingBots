using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    [SerializeField] private bool _isBuilder = true;

    private Mover _mover;
    private bool _isBuilding;

    public bool IsBuilder => _isBuilder;
    public bool IsBuilding => _isBuilding;

    public event Action<Builder> BuildStarted;
    public event Action<Builder> BuildCompleted;
    public event Action<Vector3> MovementRequested;

    public void StartBuildMission(Vector3 buildPosition, Action onBuildComplete)
    {
        if (_isBuilder == false || _isBuilding)
            return;

        _isBuilding = true;

        BuildStarted?.Invoke(this);
        MovementRequested?.Invoke(buildPosition);

        StartCoroutine(BuildProcessRoutine(onBuildComplete));
    }

    public void OnReachedBuildSite()
    {
        if (_isBuilding)
            StartCoroutine(ConstructionRoutine());
    }

    public void CancelBuild()
    {
        if (_isBuilding)
        {
            _isBuilding = false;
            StopAllCoroutines();
        }
    }

    private IEnumerator BuildProcessRoutine(Action onBuildComplete)
    {
        yield return new WaitUntil(() => _isBuilding == false);

        onBuildComplete?.Invoke();
        BuildCompleted?.Invoke(this);
    }

    private IEnumerator ConstructionRoutine()
    {
        yield return new WaitForSeconds(1f);

        _isBuilding = false;
    }
}