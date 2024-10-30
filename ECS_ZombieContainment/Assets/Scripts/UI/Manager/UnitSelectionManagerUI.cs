using UnityEngine;

public class UnitSelectionManagerUI : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;


   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UnitSelectionManager.Instance.OnSelectionAreaStart += Instance_OnSelectionAreaStart;
        UnitSelectionManager.Instance.OnSelectionAreaEnd += Instance_OnSelectionAreaEnd;

        rectTransform.gameObject.SetActive(false);

    }

    private void Instance_OnSelectionAreaStart(object sender, System.EventArgs e)
    {
        rectTransform.gameObject.SetActive(true);

        UpdateVisual();
    }

    private void Instance_OnSelectionAreaEnd(object sender, System.EventArgs e)
    {
        rectTransform.gameObject.SetActive(false);
    }

    
    // Update is called once per frame
    void Update()
    {
        if(rectTransform.gameObject.activeSelf)
        {
            UpdateVisual();
        }
        
    }

    void UpdateVisual()
    {
        Rect selectionRect = UnitSelectionManager.Instance.GetSelectionAreaRect();

        rectTransform.anchoredPosition = new Vector2(selectionRect.x, selectionRect.y);
        rectTransform.sizeDelta = new Vector2(selectionRect.width, selectionRect.height);
    }
}
