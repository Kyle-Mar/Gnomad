using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryCell : MonoBehaviour
{
    public delegate void InventoryCellClickedDelegate(InventoryCell cell);
    public InventoryCellClickedDelegate OnCellClicked;
    public void DoOnClick()
    {
        OnCellClicked?.Invoke(this);
    }


    public void UpdateTransform(Vector3 scale, Vector3 position)
    {
        gameObject.transform.localScale = scale;
        gameObject.transform.localPosition = position;
    }
}
