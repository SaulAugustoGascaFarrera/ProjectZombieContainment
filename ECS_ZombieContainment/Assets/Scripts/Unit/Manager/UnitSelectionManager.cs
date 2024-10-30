using System;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance;

    public event EventHandler OnSelectionAreaStart;
    public event EventHandler OnSelectionAreaEnd;

    private Vector2 selectionStartMousePosition;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("There is UnitSelectionManager Instance");
        }

        Instance = this;
    }


    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            selectionStartMousePosition = Input.mousePosition;
            OnSelectionAreaStart?.Invoke(this, EventArgs.Empty);
        }

        if(Input.GetMouseButtonUp(0))
        {
            OnSelectionAreaEnd?.Invoke(this, EventArgs.Empty);
        }

        if(Input.GetMouseButton(1))
        {

        }
    }

    public Rect GetSelectionAreaRect()
    {
        Vector2 selectionEndMousePosition = Input.mousePosition;

        Vector2 lowerLeftCorner = new Vector2(Mathf.Min(selectionStartMousePosition.x, selectionEndMousePosition.x), Mathf.Min(selectionStartMousePosition.y,selectionEndMousePosition.y));

        Vector2 upperRightCorner = new Vector2(Mathf.Max(selectionStartMousePosition.x,selectionEndMousePosition.x),Mathf.Max(selectionStartMousePosition.y,selectionEndMousePosition.y));

        Rect selectionRect = new Rect(lowerLeftCorner.x,lowerLeftCorner.y,upperRightCorner.x - lowerLeftCorner.x,upperRightCorner.y - lowerLeftCorner.y);

        return selectionRect;
    }



}
