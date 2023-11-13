using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using Gnomad.Utils;

namespace Entities.Player.Inventory {

    [RequireComponent(typeof(PlayerStateMachine))]
    public class PlayerInventory : MonoBehaviour
    {
        #region IconPrefabs
        [SerializeField] Object lockedCellPrefab;
        [SerializeField] Object emptyCellPrefab;
        [SerializeField] Object itemPrefab;
        [SerializeField] Object selectedCellIcon;
        #endregion
        #region UIRaycasting
        [SerializeField] GraphicRaycaster graphicRaycaster;
        [SerializeField] EventSystem eventSystem;
        #endregion
        #region PlayerInput
        PlayerControls controls;
        GameObject cursor;
        Vector2Int cursorPosition;
        #endregion
        #region Canvas Stuff
        [SerializeField] RectTransform backpackRectTransform;
        [SerializeField] GameObject backpack;
        [SerializeField] Canvas canvas;
        #endregion
        [SerializeField] bool isOpen;
        [SerializeField] BaseItem jasonItem;
        List<GameObject> cellList;
        List<GameObject> itemList;
        public bool IsOpen { get; private set; }

        InventoryData data; 

        private void Awake()
        {
            data = new InventoryData(new(3, 3, (int)Grid.CellStatus.Empty)).SetJasonItem(jasonItem);
            cellList = new();
            itemList = new();
/*            data[0, 0] = (int)Grid.CellStatus.Empty;
            data[1, 0] = (int)Grid.CellStatus.Empty;
            data[1, 1] = (int)Grid.CellStatus.Empty;*/

            backpackRectTransform = backpack.GetComponent<RectTransform>();
            if(data.PlaceItem(jasonItem, new(0, 0)))
            {
                
            }
            else
            {
            }


            // Initialize the inventory
            RedrawInventoryToCanvas();
            CloseInventory();
        }

        private void Start()
        {
            eventSystem = FindObjectOfType(typeof(EventSystem)) as EventSystem;
            controls = GetComponent<PlayerStateMachine>().Controls;
        }

        private void Update()
        {
            OpenCloseInput();
            if (isOpen)
            {
                PickupPlaceInput();
                CursorMoveInput();
                RotateInput();
                UpdateInventoryAlreadyOnCanvas();
            }
        }
        
        void OpenCloseInput()
        {
            if (!isOpen && controls.Inventory.OpenClose.WasPressedThisFrame())
            {
                isOpen = true;
                OpenInventory();

            }
            else if (isOpen && controls.Inventory.OpenClose.WasPressedThisFrame())
            {
                isOpen = false;
                CloseInventory();
            }
        }

        void CursorMoveInput()
        {
            // I have no idea why this is so reversed but it is so deal with it.
            if (controls.Inventory.MoveCursorX.WasPressedThisFrame())
            {
                float inputX = controls.Inventory.MoveCursorX.ReadValue<Vector2>().x;
                Debug.Log(inputX);
                if (inputX > 0)
                {
                    cursorPosition.x = Mathf.Clamp(cursorPosition.x + 1, 0, data.NumColumns - 1);
                }
                else if (inputX < 0)
                {
                    cursorPosition.x = Mathf.Clamp(cursorPosition.x - 1, 0, data.NumColumns - 1);
                }
                goto moveItem;
            }
            if (controls.Inventory.MoveCursorY.WasPressedThisFrame())
            {
                float inputY = controls.Inventory.MoveCursorY.ReadValue<Vector2>().y;
                Debug.Log(inputY);
                if (inputY > 0)
                {
                    cursorPosition.y = Mathf.Clamp(cursorPosition.y - 1, 0, data.NumColumns - 1);
                }
                else if (inputY < 0)
                {
                    cursorPosition.y = Mathf.Clamp(cursorPosition.y + 1, 0, data.NumColumns - 1);
                }
                goto moveItem;
            }

            return;
            moveItem:
            if(data.CurrentItem is not null)
            {
                var panelOffsetX = backpackRectTransform.rect.width / data.NumColumns;
                var panelOffsetY = backpackRectTransform.rect.height / data.NumRows;
                InventoryItem item = data.CurrentItem.GetComponent<InventoryItem>();
                Debug.Log(cursorPosition);
                item.UpdateIMG(item.Item, new Vector2(panelOffsetX, panelOffsetY), cursorPosition, backpackRectTransform);
            }
        }

        void PickupPlaceInput()
        {
            if (!controls.Inventory.PickupPlace.WasPressedThisFrame())
            {
                return;
            }
            if (data.CurrentItem is not null)
            {
                Vector2Int itemPosition = new(cursorPosition.x, cursorPosition.y);
                Debug.Log(data.CurrentItem.name);
                InventoryItem invItem = data.CurrentItem.GetComponent<InventoryItem>();
                BaseItem item = invItem.Item;
                if (data.PlaceItem(item, itemPosition))
                {
                    var panelOffsetX = backpackRectTransform.rect.width / data.NumColumns;
                    var panelOffsetY = backpackRectTransform.rect.height / data.NumRows;
                    //invItem.UpdateIMG(item, new(panelOffsetX, panelOffsetY), cursorPosition, backpackRectTransform);
                    data.SetCurrentItem(null);
                }
                return;
            }
            else
            {
                PointerEventData pointerEventData = new(eventSystem);
                pointerEventData.position = cursor.transform.position;
                List<RaycastResult> results = new();
                graphicRaycaster.Raycast(pointerEventData, results);

                foreach (var x in results)
                {
                    InventoryItem item;
                    if (x.gameObject.TryGetComponent(out item))
                    {
                        Vector2Int itemPosition = data.ItemPositions[item.Item];
                        data.TryRemoveItem(item.Item, itemPosition);
                        data.ItemPositions.Remove(item.Item);
                        cursorPosition = itemPosition;
                        //Debug.Log(itemPosition);
                        //x.gameObject.transform.localScale *= 1.2f;
                        data.SetCurrentItem(x.gameObject);
                        break;
                    }
                }
            }
        }

        void RotateInput()
        {
            if (controls.Inventory.RotateRight.WasPressedThisFrame())
            {
                if(data.CurrentItem == null)
                {
                    return;
                }





                InventoryItem item;
                if(data.CurrentItem.TryGetComponent(out item)){
                    if(item.Rotate90(data.Grid) == false)
                    {
                        return;
                    }
                }
                var panelWidth = 1f / data.NumColumns;
                var panelHeight = 1f / data.NumRows;

                var backpackTopLeftCorner = GetBackpackTopLeftCorner();

                var panelOffsetX = backpackRectTransform.rect.width / data.NumColumns;
                var panelOffsetY = backpackRectTransform.rect.height / data.NumRows;
                //cursorPosition = cursorPosition.Rotate(90f);
                               
                data.CurrentItem.transform.Rotate(Vector3.forward * -90f);

            }
        }
        
        //If marked dirty, we'll need to do a full refresh?
        void UpdateInventoryAlreadyOnCanvas()
        {
            var panelWidth = 1f / data.NumColumns;
            var panelHeight = 1f / data.NumRows;
            var panelOffsetX = backpackRectTransform.rect.height / data.NumColumns;
            var panelOffsetY = backpackRectTransform.rect.height / data.NumRows;
            var backpackTopLeftCorner = GetBackpackTopLeftCorner();

            foreach (var x in data.Grid)
            {
                var panel = cellList[GetIndex(x.row, x.col)];
                panel.transform.localScale = new Vector3(panelWidth, panelHeight, 1);
                panel.transform.localPosition = GetNewPanelLocalPosition(backpackTopLeftCorner, panelOffsetX, panelOffsetY, x.row, x.col);
            }
            cursor.transform.localPosition = GetNewPanelLocalPosition(backpackTopLeftCorner, panelOffsetX, panelOffsetY, cursorPosition.y, cursorPosition.x);
        }

        public void RedrawInventoryToCanvas()
        {
            foreach (var x in cellList)
            {
                Destroy(x);
            }
            foreach (var x in itemList)
            {
                Destroy(x);
            }
            cellList.Clear();
            itemList.Clear();

            var panelWidth = 1f / data.NumColumns;
            var panelHeight = 1f / data.NumRows;

            var backpackTopLeftCorner = GetBackpackTopLeftCorner();

            var panelOffsetX = backpackRectTransform.rect.width / data.NumColumns;
            var panelOffsetY = backpackRectTransform.rect.height / data.NumRows;

            foreach (var x in data.Grid)
            {
                // Instantiate the correct cell ui element based off of the value in the grid.
                GameObject cell;
                if (data[x.row, x.col] == (int)Grid.CellStatus.Locked)
                {
                    cell = Instantiate(lockedCellPrefab) as GameObject;
                }
                else
                {
                    cell = Instantiate(emptyCellPrefab) as GameObject;
                }
                cell.transform.SetParent(backpack.transform, false);

                // set callback and update scale and position
                InventoryCell ic;
                if (cell.TryGetComponent(out ic))
                {
                    ic.r = x.row;
                    ic.c = x.col;
                    ic.UpdateTransform(new Vector3(panelWidth, panelHeight, 1), GetNewPanelLocalPosition(backpackTopLeftCorner, panelOffsetX, panelOffsetY, x.row, x.col));
                }
                cellList.Add(cell);
            }

            foreach(var kvp in data.ItemPositions)
            {
                var item = Instantiate(itemPrefab) as GameObject;
                var baseItem = kvp.Key;
                item.GetComponent<InventoryItem>().Initialize(baseItem, baseItem.grid, new Vector2(panelWidth, panelHeight), new Vector2(panelOffsetX, panelOffsetY), backpackRectTransform, backpack.transform, kvp.Value);
                itemList.Add(item);
            }

            GameObject cur = Instantiate(selectedCellIcon) as GameObject;
            cur.transform.SetParent(backpack.transform, false);
            cur.transform.localScale = new(panelWidth, panelHeight, 1);
            Vector2Int cursorPos = GetCursorIndexOnGrid();
            cursorPosition = cursorPos;
            cur.transform.localPosition = GetNewPanelLocalPosition(backpackTopLeftCorner, panelOffsetX, panelOffsetY, cursorPos.x, cursorPos.y);
            cursor = cur;
        }

        /*
        This is inefficent: 
        it could be done like this:
        -Canvas
            -CellGroup : Call setactive here.
                -All the cells
         
        */
        void CloseInventory()
        {
            foreach (var x in cellList)
            {
                x.SetActive(false);
            }
            foreach (var x in itemList)
            {
                x.SetActive(false);
            }
            data.SetCurrentItem(null);
            cursor.SetActive(false);
            cursorPosition = GetCursorIndexOnGrid();
        }

        void OpenInventory()
        {
            foreach (var x in cellList)
            {
                x.SetActive(true);
            }
            foreach (var x in itemList)
            {
                x.SetActive(true);
            }
            cursor.SetActive(true);
        }

        Vector3 GetNewPanelLocalPosition(Vector3 backpackTopLeftCorner, float panelOffsetX, float panelOffsetY, int row, int col)
        {
            return new Vector3(backpackTopLeftCorner.x + panelOffsetX * col + panelOffsetX / 2, backpackTopLeftCorner.y - panelOffsetY * row - panelOffsetY / 2, 1);
        }

        public static Vector3 GetNewItemLocalPosition(Vector3 backpackTopLeftCorner, Vector2 itemSize, Vector2 cellSize, Vector2 pivot, int row, int col)
        {
            return new Vector3(backpackTopLeftCorner.x + (cellSize.x * row) + (itemSize.x * pivot.x), backpackTopLeftCorner.y - (cellSize.y * col) + (itemSize.y * pivot.y ) - itemSize.y, 1);
            //+cellSize.x * col + itemSize.x / 2
        }

        Vector3 GetBackpackTopLeftCorner()
        {
            Vector3[] arr = new Vector3[4];
            backpackRectTransform.GetLocalCorners(arr);
            return arr[1];
        }

        public static Vector3 GetBackpackTopLeftCorner(RectTransform rectT)
        {
            Vector3[] arr = new Vector3[4];
            rectT.GetLocalCorners(arr);
            return arr[1];
        }

        // put in middle of grid.
        Vector2Int GetCursorIndexOnGrid()
        {
            int r = data.NumColumns / 2;
            int c = data.NumRows / 2;

            return new(r, c);
        }
        int GetIndex(int r, int c)
        {
            return r * data.NumColumns + c;
        }
        public void DoAThing(InventoryCell cell)
        {
            Debug.Log("I HAVE BEEN CLICKED");
        }
    }
}