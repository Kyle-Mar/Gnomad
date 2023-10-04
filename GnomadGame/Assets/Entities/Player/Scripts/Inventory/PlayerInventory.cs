using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

namespace PlayerInventory {

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
        
        [SerializeField] BaseItem jasonItem;//temporary. we will get it from the other part of the inventory soon.

        [SerializeField] bool isOpen;
        List<GameObject> cellList;
        List<GameObject> itemList;
        Grid grid;
        GameObject? currentItem = null;
        Dictionary<BaseItem, Vector2Int> itemPositions = new();

        public bool IsOpen => isOpen;


        int this[int i, int j]
        {
            get => grid[i, j];
            set
            {
                grid[i, j] = value;
            }
        }

        private void Awake()
        {
            grid = new(3, 3, (int)Grid.CellStatus.Locked);
            this[0, 0] = (int)Grid.CellStatus.Empty;
            this[1, 0] = (int)Grid.CellStatus.Empty;
            this[1, 1] = (int)Grid.CellStatus.Empty;
            cellList = new();
            itemList = new();
            backpackRectTransform = backpack.GetComponent<RectTransform>();
            if(TryPlaceItem(jasonItem, new(0, 1)))
            {
                Debug.Log("HELLO WORLD");
            }

            foreach (var x in grid)
            {
                //Debug.Log(x.value + " " + x.row + " " + x.col);
            }

            // Initialize the inventory
            //RedrawInventoryToCanvas();
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
                CursorMoveInput();
                PickupPlaceInput();
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
                    cursorPosition.x = Mathf.Clamp(cursorPosition.x + 1, 0, grid.NumColumns - 1);
                }
                else if (inputX < 0)
                {
                    cursorPosition.x = Mathf.Clamp(cursorPosition.x - 1, 0, grid.NumColumns - 1);
                }

            }
            if (controls.Inventory.MoveCursorY.WasPressedThisFrame())
            {
                float inputY = controls.Inventory.MoveCursorY.ReadValue<Vector2>().y;
                Debug.Log(inputY);
                if (inputY > 0)
                {
                    cursorPosition.y = Mathf.Clamp(cursorPosition.y - 1, 0, grid.NumColumns - 1);
                }
                else if (inputY < 0)
                {
                    cursorPosition.y = Mathf.Clamp(cursorPosition.y + 1, 0, grid.NumColumns - 1);
                }

            }
            
        }

        void PickupPlaceInput()
        {
            if (controls.Inventory.PickupPlace.WasPressedThisFrame())
            {
                Vector2Int itemPosition = new(cursorPosition.x, cursorPosition.y);
                if (currentItem is not null)
                {
                    Debug.Log(currentItem.name);
                    BaseItem item = currentItem.GetComponent<InventoryItem>().Item;
                    Debug.Log(CheckPlaceItem(item, itemPosition));
                    if(CheckPlaceItem(item, itemPosition))
                    {
                        Debug.Log(itemPosition);
                        TryPlaceItem(item, itemPosition);
                        currentItem = null;
                    }
                    return;
                }

                PointerEventData pointerEventData = new(eventSystem);
                pointerEventData.position = cursor.transform.position;
                List<RaycastResult> results = new();
                graphicRaycaster.Raycast(pointerEventData, results);

                foreach (var x in results)
                {

                    InventoryItem item;
                    if(x.gameObject.TryGetComponent(out item))
                    {
                        TryRemoveItem(item.Item, itemPositions[item.Item]);
                        itemPositions.Remove(item.Item);
                        //Debug.Log(cursorPosition);
                        //cursorPosition = itemPosition;
                        //Debug.Log(itemPosition);
                        x.gameObject.transform.localScale *= 1.2f;
                        currentItem = x.gameObject;
                        break;
                    }
                }
            }
        }

        void RotateInput()
        {
            if (controls.Inventory.RotateRight.WasPressedThisFrame())
            {
                if(currentItem == null)
                {
                    return;
                }
                InventoryItem item;
                if(currentItem.TryGetComponent(out item)){
                    item.Rotate90();
                }
                currentItem.transform.Rotate(new Vector3(0, 0, 90f));
            }
        }
        
        //If marked dirty, we'll need to do a full refresh?
        void UpdateInventoryAlreadyOnCanvas()
        {
            var panelWidth = 1f / grid.NumColumns;
            var panelHeight = 1f / grid.NumRows;
            var panelOffsetX = backpackRectTransform.rect.height / grid.NumColumns;
            var panelOffsetY = backpackRectTransform.rect.height / grid.NumRows;
            var backpackTopLeftCorner = GetBackpackTopLeftCorner();

            foreach (var x in grid)
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

            var panelWidth = 1f / grid.NumColumns;
            var panelHeight = 1f / grid.NumRows;

            var backpackTopLeftCorner = GetBackpackTopLeftCorner();

            var panelOffsetX = backpackRectTransform.rect.width / grid.NumColumns;
            var panelOffsetY = backpackRectTransform.rect.height / grid.NumRows;

            foreach (var x in grid)
            {
                // Instantiate the correct cell ui element based off of the value in the grid.
                GameObject cell;
                if (this[x.row, x.col] == (int)Grid.CellStatus.Locked)
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
                    ic.UpdateTransform(new Vector3(panelWidth, panelHeight, 1), GetNewPanelLocalPosition(backpackTopLeftCorner, panelOffsetX, panelOffsetY, x.row, x.col));
                }
                cellList.Add(cell);
            }

            foreach(var kvp in itemPositions)
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
            currentItem = null;
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
        }

        public bool CheckPlaceItem(BaseItem item, Vector2Int desiredPos)
        {
            Grid collGrid = item.GetGrid();
            grid.OutputTXT();
            return (!grid.CheckCollisionWithGrid(ref collGrid, desiredPos));
        }
        public bool TryPlaceItem(BaseItem item, Vector2Int desiredPos)
        {
            Grid collGrid = item.GetGrid();

            grid.OutputTXT();
            if (grid.CheckCollisionWithGrid(ref collGrid, desiredPos))
            {
                return false;
            }
            //Debug.Log("success");
            for (int r = desiredPos.y; r < collGrid.NumRows + desiredPos.y; r++)
            {
                for (int c = desiredPos.x; c < collGrid.NumColumns + desiredPos.x; c++)
                {
                    if (collGrid[r - desiredPos.y, c - desiredPos.x] != (int)Grid.CellStatus.Empty)
                    {
                        grid[r,c] = collGrid[r - desiredPos.y, c - desiredPos.x];
                    }
                }
            }
            Debug.Log(desiredPos);
            itemPositions.Add(item, desiredPos);
            RedrawInventoryToCanvas();
            return true;
        }

        public bool TryRemoveItem(BaseItem item, Vector2Int desiredPos)
        {
            Grid collGrid = item.GetGrid();
            grid.OutputTXT();
            collGrid.OutputTXT();
            for (int r = desiredPos.y; r < collGrid.NumRows + desiredPos.y; r++)
            {
                for (int c = desiredPos.x; c < collGrid.NumRows + desiredPos.x; c++)
                {
                    if (collGrid[r - desiredPos.y, c - desiredPos.x] != (int)Grid.CellStatus.Empty)
                    {
                        grid[r, c] = (int)Grid.CellStatus.Empty;
                        grid.OutputTXT();
                    }
                }
            }
            //grid.OutputTXT();
            return true;
        }

        Vector3 GetNewPanelLocalPosition(Vector3 backpackTopLeftCorner, float panelOffsetX, float panelOffsetY, int row, int col)
        {
            return new Vector3(backpackTopLeftCorner.x + panelOffsetX * col + panelOffsetX / 2, backpackTopLeftCorner.y - panelOffsetY * row - panelOffsetY / 2, 1);
        }

        public static Vector3 GetNewItemLocalPosition(Vector3 backpackTopLeftCorner, Vector2 itemSize, Vector2 cellSize, int row, int col)
        {
            Debug.Log(cellSize);
            return new Vector3(backpackTopLeftCorner.x + cellSize.x * row + itemSize.x / 2, backpackTopLeftCorner.y - cellSize.y * col - itemSize.y / 2, 1);
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
            int r = grid.NumColumns / 2;
            int c = grid.NumRows / 2;

            return new(r, c);
        }
        int GetIndex(int r, int c)
        {
            return r * grid.NumColumns + c;
        }
        public void DoAThing(InventoryCell cell)
        {
            Debug.Log("I HAVE BEEN CLICKED");
        }
    }
}