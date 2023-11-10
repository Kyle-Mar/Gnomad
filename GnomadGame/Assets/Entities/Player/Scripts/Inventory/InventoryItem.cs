using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Gnomad.Utils;

namespace Entities.Player.Inventory
{
    [RequireComponent(typeof(Image))]
    public class InventoryItem : MonoBehaviour
    {
        [SerializeField] BaseItem item;
        [SerializeField] Grid grid;
        public BaseItem Item => item;
        public Grid Grid => grid;
        Vector3 pvt;
        int rot;

        public void Initialize(BaseItem item, Grid grid, Vector2 panelSize, Vector2 panelOffset, RectTransform backpackRectTransform, Transform backpackGOTransform, Vector2Int itemPos)
        {
            this.item = item;
            this.grid = (Grid)grid.Clone();

            transform.SetParent(backpackGOTransform, false);
            
            transform.localScale = new Vector3(panelSize.x * grid.NumColumns, panelSize.y * grid.NumRows, 1);
            
            gameObject.GetComponent<Image>().sprite = Sprite.Create(item.itemTexture, new(0, 0, item.itemTexture.width, item.itemTexture.height), new(.5f, .5f));
            
            Vector2 itemSize = new(backpackRectTransform.rect.width * transform.localScale.x, backpackRectTransform.rect.height * transform.localScale.y);

            RectTransform itemTransform = transform as RectTransform;
            //itemTransform.pivot = new(1.0f / grid.NumColumns / 2, 1.0f / grid.NumRows / 2);
            itemTransform.localPosition = PlayerInventory.GetNewItemLocalPosition(PlayerInventory.GetBackpackTopLeftCorner(backpackRectTransform), itemSize, panelOffset, itemTransform.pivot, itemPos.x, itemPos.y);
            Vector3[] arr = new Vector3[4];
            itemTransform.GetWorldCorners(arr);
            pvt = arr[1];
            rot = 1;
            //Debug.Log(PlayerInventory.GetNewItemLocalPosition(PlayerInventory.GetBackpackTopLeftCorner(backpackRectTransform), itemSize, panelSize, 0, 0));
        }

        public void Rotate90(GraphicRaycaster graphicRaycaster, EventSystem eventSystem)
        {
            grid.ReverseColumns();
            grid.Transpose();
            RectTransform rectTransform = transform as RectTransform;
            PointerEventData pointerEventData = new(eventSystem);
            Vector3[] arr = new Vector3[4];
            rectTransform.GetWorldCorners(arr);
            pointerEventData.position = arr[1];


            
            List<RaycastResult> results = new();
            graphicRaycaster.Raycast(pointerEventData, results);
            Debug.DrawRay(arr[rot], Vector3.right * 100f, Color.red, 10f);
            rot -= 1;
            if (rot < 0)
            {
                rot = 3;
            }
            foreach (var x in results)
            {
                InventoryCell cell;
                if (x.gameObject.TryGetComponent(out cell))
                {
                    Debug.Log(cell.r + " " + cell.c);
                }
            }
        }

        public void UpdateIMG(BaseItem item, Vector2 panelOffset, Vector2Int itemPos, RectTransform backpackRectTransform)
        {
            Vector2 itemSize = new(backpackRectTransform.rect.width * transform.localScale.x, backpackRectTransform.rect.height * transform.localScale.y);
            RectTransform itemTransform = transform as RectTransform;
            //itemTransform.pivot = new(1.0f / grid.NumColumns / 2, 1.0f / grid.NumRows / 2);
            itemTransform.localPosition = PlayerInventory.GetNewItemLocalPosition(PlayerInventory.GetBackpackTopLeftCorner(backpackRectTransform), itemSize, panelOffset, itemTransform.pivot, itemPos.x, itemPos.y);
        }
    }
}
