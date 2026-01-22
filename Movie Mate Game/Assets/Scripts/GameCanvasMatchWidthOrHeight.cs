using UnityEngine;
using UnityEngine.UI;
using 

[RequireComponent(typeof(CanvasScaler))]
public class GameCanvasMatchWidthOrHeight : MonoBehaviour
{
    [Range(0f,1f)] [SerializeField] private float phoneMatch = 0;
    [Range(0f, 1f)] [SerializeField] private float tabletMatch = 1;
    
    private CanvasScaler _canvasScaler;

    private void Awake()
    {
        _canvasScaler = GetComponent<CanvasScaler>();

        UpdateView();
    }


    private void UpdateView()
    {
        // _canvasScaler.matchWidthOrHeight = tabletMatch : phoneMatch;
    }
}
