using UnityEngine;
using UnityEngine.UI;

public class BuildUI : MonoBehaviour
{
    [SerializeField] private Button _buildButton;
    [SerializeField] private Button _cancelButton;

    private BuildSystem _buildSystem;
    private Vector3 _worldPosition;

    public void Initialize(BuildSystem buildSystem, Vector3 worldPosition)
    {
        _buildSystem = buildSystem;
        _worldPosition = worldPosition;

        SetupButtons();
        Show();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void SetupButtons()
    {
        _buildButton.onClick.RemoveAllListeners();
        _cancelButton.onClick.RemoveAllListeners();

        _buildButton.onClick.AddListener(OnBuildClicked);
        _cancelButton.onClick.AddListener(OnCancelClicked);
    }

    private void OnBuildClicked()
    {
        _buildSystem.OnBuildClicked();
    }

    private void OnCancelClicked()
    {
        _buildSystem.OnCancelClicked();
    }
}