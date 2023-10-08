using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryCell : MonoBehaviour
{
    public delegate void InventoryCellClickedDelegate(InventoryCell cell);
    public InventoryCellClickedDelegate OnCellClicked;
    public int r, c;
    public void DoOnClick()
    {
        OnCellClicked?.Invoke(this);
    }

    void Initialize(int r, int c)
    {
        this.r = r; this.c = c;
    }
    public void UpdateTransform(Vector3 scale, Vector3 position)
    {
        gameObject.transform.localScale = scale;
        gameObject.transform.localPosition = position;
    }
}
