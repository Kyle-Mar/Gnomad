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
        public Vector2Int ItemPos => itemPos;
        RectTransform backpackRectTransform;
        Vector2Int itemPos;

        public Grid Grid => grid;
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
/*          Vector3[] arr = new Vector3[4];
            itemTransform.GetWorldCorners(arr);
            pvt = arr[1];*/
            rot = 1;
            this.backpackRectTransform = backpackRectTransform;
            this.itemPos = itemPos;
            //Debug.Log(PlayerInventory.GetNewItemLocalPosition(PlayerInventory.GetBackpackTopLeftCorner(backpackRectTransform), itemSize, panelSize, 0, 0));
        }

        public bool Rotate90(Grid inventoryGrid)
        {
            Grid nextGrid = (Grid)grid.Clone();
            nextGrid.ReverseColumns();
            nextGrid.Transpose();

            Vector2Int nextItemPos = itemPos;
            nextItemPos += item.RotationMovementSequence[rot];
            rot += 1;
            if (rot > 3){
                rot = 0;
            }

            itemPos = nextItemPos;

            return true;
        }

        public void UpdateIMG(BaseItem item, Vector2 panelOffset, Vector2Int itemPos, RectTransform backpackRectTransform)
        {
            Vector2 itemSize = new(backpackRectTransform.rect.width * transform.localScale.x, backpackRectTransform.rect.height * transform.localScale.y);
            //RectTransform itemTransform = transform as RectTransform;
            //itemTransform.localPosition = PlayerInventory.GetNewItemLocalPosition(PlayerInventory.GetBackpackTopLeftCorner(backpackRectTransform), itemSize, panelOffset, itemTransform.pivot, itemPos.x, itemPos.y);
            //Debug.Log(itemTransform.localPosition);
            this.itemPos = itemPos;
        }
    }
}
