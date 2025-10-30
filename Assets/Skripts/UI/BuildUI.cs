using UnityEngine;
using UnityEngine.UI;

public class BuildUI : MonoBehaviour
{
   /* [SerializeField] private Button _buildButton;
    [SerializeField] private Button _cancelButton;

    private BaseBuilder _baseBuilder;
    private Vector3 _worldPosition;

    public void Initialize(BaseBuilder baseBuilder, Vector3 worldPosition)
    {
        _baseBuilder = baseBuilder;
        _worldPosition = worldPosition;

        SetupButtons();
        PositionUI();
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

    private void PositionUI()
    {
        Vector3 uiPosition = _worldPosition + new Vector3(0, 3f, 0);
        transform.position = uiPosition;

        //LookAtCamera();
    }

    private void LookAtCamera()
    {
        if (Camera.main != null)
        {
            transform.LookAt(Camera.main.transform);
            Vector3 euler = transform.eulerAngles;
            euler.x = 0;
            euler.z = 0;
            transform.eulerAngles = euler;
        }
    }

    private void OnBuildClicked()
    {
        _baseBuilder.StartConstruction();
        Hide();
    }

    private void OnCancelClicked()
    {
        _baseBuilder.CancelConstruction();
        Hide();
    }*/
}