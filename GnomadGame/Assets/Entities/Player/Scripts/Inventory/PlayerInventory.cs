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
        [SerializeField] Object lockedCellPrefab;
        [SerializeField] Object emptyCellPrefab;
        [SerializeField] Object itemPrefab;
        [SerializeField] Object selectedCellIcon;
        [SerializeField] BaseItem jasonItem;
        [SerializeField] Canvas canvas;
        [SerializeField] GameObject backpack;
        [SerializeField] bool isOpen;
        [SerializeField] RectTransform backpackRectTransform;
        PlayerControls controls;
        List<GameObject> cellList;
        List<GameObject> itemList;
        GameObject cursor;
        Vector2Int cursorPosition;
        Grid grid;
        Dictionary<BaseItem, Vector2Int> itemPositions = new();
        [SerializeField] GraphicRaycaster graphicRaycaster;
        [SerializeField] EventSystem eventSystem;

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
            //var testObject = gameObject.AddComponent<TestItem>();
            grid = new(3, 3, (int)Grid.CellStatus.Locked);
            this[0, 0] = (int)Grid.CellStatus.Empty;
            this[1, 0] = (int)Grid.CellStatus.Empty;
            this[1, 1] = (int)Grid.CellStatus.Empty;
            cellList = new();
            itemList = new();
            //itemPositions.Add(jasonItem, new(0, 0));
            backpackRectTransform = backpack.GetComponent<RectTransform>();
            if(TryPlaceItem(jasonItem, new(0, 0)))
            {
                Debug.Log("HELLO WORLD");
            }

            foreach (var x in grid)
            {
                Debug.Log(x.value + " " + x.row + " " + x.col);
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
            if(!isOpen && controls.Inventory.OpenClose.WasPressedThisFrame())
            {
                isOpen = true;
                OpenInventory();
                
            }
            else if(isOpen && controls.Inventory.OpenClose.WasPressedThisFrame())
            {
                isOpen = false;
                CloseInventory();
            }

            if (isOpen)
            {
                //Vector2 invMoveVector = controls.Player.Move.ReadValue<Vector2>();

                // I have no idea why this is so reversed but it is so deal with it.
                if (controls.Inventory.MoveCursorX.WasPressedThisFrame())
                {
                    float inputX = controls.Inventory.MoveCursorX.ReadValue<Vector2>().x;
                    Debug.Log(inputX);
                    if (inputX > 0)
                    {
                        cursorPosition.x = Mathf.Clamp(cursorPosition.x + 1, 0, grid.NumColumns-1);
                    }
                    else if (inputX < 0)
                    {
                        cursorPosition.x = Mathf.Clamp(cursorPosition.x - 1, 0, grid.NumColumns-1);
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


                if (controls.Inventory.PickupPlace.WasPressedThisFrame())
                {
                    PointerEventData pointerEventData = new(eventSystem);
                    pointerEventData.position = cursor.transform.position;
                    List<RaycastResult> results = new();
                    graphicRaycaster.Raycast(pointerEventData, results);

                    foreach(var x in results)
                    {
                        //if(TryGetComponent<>)
                        //Debug.Log(x.gameObject.name);
                    }
                }
                UpdateInventoryAlreadyOnCanvas();
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
                    ic.OnCellClicked += DoAThing;
                    ic.UpdateTransform(new Vector3(panelWidth, panelHeight, 1), GetNewPanelLocalPosition(backpackTopLeftCorner, panelOffsetX, panelOffsetY, x.row, x.col));
                }
                cellList.Add(cell);
            }

            foreach(var kvp in itemPositions)
            {
                var item = Instantiate(itemPrefab) as GameObject;
                var baseItem = kvp.Key;
                item.GetComponent<Image>().sprite = Sprite.Create(baseItem.itemTexture, new(0, 0, baseItem.itemTexture.width , baseItem.itemTexture.height), new(.5f,.5f));
                item.transform.SetParent(backpack.transform, false);
                item.transform.localScale = new Vector3(panelWidth * baseItem.grid.NumColumns, panelHeight * baseItem.grid.NumRows, 1);
                var itemOffsetX = backpackRectTransform.rect.width * item.transform.localScale.x;
                var itemOffsetY = backpackRectTransform.rect.height * item.transform.localScale.y;
                item.transform.localPosition = GetNewItemLocalPosition(backpackTopLeftCorner, itemOffsetX, itemOffsetY, panelOffsetX, panelOffsetY,kvp.Value.x, kvp.Value.y);
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


        public bool TryPlaceItem(BaseItem item, Vector2Int desiredPos)
        {
            Grid collGrid = item.GetGrid();
            if (grid.CheckCollisionWithGrid(ref collGrid, desiredPos))
            {
                return false;
            }
            for (int i = desiredPos.x; i < collGrid.NumRows + desiredPos.x; i++)
            {
                for (int j = desiredPos.y; j < collGrid.NumColumns + desiredPos.y; j++)
                {
                    if (collGrid[i - desiredPos.x, j - desiredPos.y] != (int)Grid.CellStatus.Empty)
                    {
                        grid[i, j] = collGrid[i - desiredPos.x, j - desiredPos.y];
                    }
                }
            }
            itemPositions.Add(item, desiredPos);
            RedrawInventoryToCanvas();
            return true;
        }

        Vector3 GetNewPanelLocalPosition(Vector3 backpackTopLeftCorner, float panelOffsetX, float panelOffsetY, int row, int col)
        {
            return new Vector3(backpackTopLeftCorner.x + panelOffsetX * col + panelOffsetX / 2, backpackTopLeftCorner.y - panelOffsetY * row - panelOffsetY / 2, 1);
        }

        Vector3 GetNewItemLocalPosition(Vector3 backpackTopLeftCorner, float itemSizeX, float itemSizeY, float cellSizeX, float cellSizeY, int row, int col)
        {
            return new Vector3(backpackTopLeftCorner.x + cellSizeX * col + itemSizeX / 2, backpackTopLeftCorner.y - cellSizeY * row - itemSizeY / 2, 1);

        }

        Vector3 GetBackpackTopLeftCorner()
        {
            Vector3[] arr = new Vector3[4];
            backpackRectTransform.GetLocalCorners(arr);
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