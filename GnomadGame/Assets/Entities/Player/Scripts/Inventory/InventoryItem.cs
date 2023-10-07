using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Entities.Player.Inventory
{
    [RequireComponent(typeof(Image))]
    public class InventoryItem : MonoBehaviour
    {
        [SerializeField] BaseItem item;
        [SerializeField] Grid grid;
        public BaseItem Item => item;
        public Grid Grid => grid;

        public void Initialize(BaseItem item, Grid grid, Vector2 panelSize, Vector2 panelOffset, RectTransform backpackRectTransform, Transform backpackGOTransform, Vector2Int itemPos)
        {
            this.item = item;
            this.grid = grid;

            transform.SetParent(backpackGOTransform, false);
            
            transform.localScale = new Vector3(panelSize.x * grid.NumColumns, panelSize.y * grid.NumRows, 1);
            
            gameObject.GetComponent<Image>().sprite = Sprite.Create(item.itemTexture, new(0, 0, item.itemTexture.width, item.itemTexture.height), new(.5f, .5f));
            
            Vector2 itemSize = new(backpackRectTransform.rect.width * transform.localScale.x, backpackRectTransform.rect.height * transform.localScale.y);

            RectTransform itemTransform = transform as RectTransform;
            itemTransform.pivot = new(1.0f / grid.NumColumns / 2, 1.0f / grid.NumRows / 2);
            itemTransform.localPosition = PlayerInventory.GetNewItemLocalPosition(PlayerInventory.GetBackpackTopLeftCorner(backpackRectTransform), itemSize, panelOffset, itemTransform.pivot, itemPos.x, itemPos.y);

            //Debug.Log(PlayerInventory.GetNewItemLocalPosition(PlayerInventory.GetBackpackTopLeftCorner(backpackRectTransform), itemSize, panelSize, 0, 0));
        }

        public void Rotate90()
        {
            grid.ReverseColumns();
            grid.Transpose();
            //grid.OutputTXT();
        }

        public void UpdateIMG(BaseItem item, Vector2 panelOffset, Vector2Int itemPos, RectTransform backpackRectTransform)
        {
            Vector2 itemSize = new(backpackRectTransform.rect.width * transform.localScale.x, backpackRectTransform.rect.height * transform.localScale.y);
            RectTransform itemTransform = transform as RectTransform;
            itemTransform.pivot = new(1.0f / grid.NumColumns / 2, 1.0f / grid.NumRows / 2);
            itemTransform.localPosition = PlayerInventory.GetNewItemLocalPosition(PlayerInventory.GetBackpackTopLeftCorner(backpackRectTransform), itemSize, panelOffset, itemTransform.pivot, itemPos.x, itemPos.y);
        }
    }
}
