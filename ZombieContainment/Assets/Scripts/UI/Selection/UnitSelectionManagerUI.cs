using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class UnitSelectionManagerUI : MonoBehaviour
{
    [SerializeField] private RectTransform selectionAreaRectTransform;
    [SerializeField] private Canvas canvas;


    private void Start()
    {
        UnitSelectionManager.Instance.OnSelectionAreaStart += Instance_OnSelectionAreaStart;
        UnitSelectionManager.Instance.OnSelectionAreaEnd += Instance_OnSelectionAreaEnd;

        selectionAreaRectTransform.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(selectionAreaRectTransform.gameObject.activeSelf)
        {
            UpdateVisual();
        }
    }

    private void Instance_OnSelectionAreaStart(object sender, System.EventArgs e)
    {
        selectionAreaRectTransform.gameObject.SetActive(true);
        UpdateVisual();
    }

    private void Instance_OnSelectionAreaEnd(object sender, System.EventArgs e)
    {
        selectionAreaRectTransform.gameObject.SetActive(false);
    }


    public void UpdateVisual()
    {
        Rect selectionAreaRect = UnitSelectionManager.Instance.GetSelectionAreaRect();

        float canvasScale = canvas.transform.localScale.x;

        selectionAreaRectTransform.anchoredPosition = new Vector2(selectionAreaRect.x, selectionAreaRect.y) / canvasScale;
        selectionAreaRectTransform.sizeDelta = new Vector2(selectionAreaRect.width, selectionAreaRect.height) / canvasScale;
    }

    
}
