using UnityEngine;

public class Flag : MonoBehaviour
{
    [SerializeField] private Renderer _flagRenderer;

    public void SetColor(Color color)
    {
        if (_flagRenderer != null)
            _flagRenderer.material.color = color;
    }
}