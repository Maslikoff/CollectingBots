using UnityEngine;

public class BuildSystem : MonoBehaviour
{
    [SerializeField] private BaseBuilder playerBase;

    private BaseBuilder _currentBaseBuilder;

    public void TryCreateBuildSite(Vector3 position)
    {
        _currentBaseBuilder = playerBase.GetComponent<BaseBuilder>();
        _currentBaseBuilder?.TryCreateBuildSite(position);
    }

    public void OnBuildClicked()
    {
        _currentBaseBuilder?.StartConstruction();
    }

    public void OnCancelClicked()
    {
        _currentBaseBuilder?.CancelConstruction();
    }
}