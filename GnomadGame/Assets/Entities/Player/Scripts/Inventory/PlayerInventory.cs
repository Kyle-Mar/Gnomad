using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace PlayerInventory {

    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] Object lockedCellPrefab;
        [SerializeField] Object emptyCellPrefab;
        [SerializeField] Object itemPrefab;
        [SerializeField] BaseItem jasonItem;
        [SerializeField] Canvas canvas;
        [SerializeField] GameObject backpack;
        RectTransform backpackRectTransform;
        List<GameObject> cellList;
        Grid grid;
        Dictionary<BaseItem, Vector2Int> itemPositions = new();

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
            grid = new(3, 4, (int)Grid.CellStatus.Locked);
            this[1, 1] = (int)Grid.CellStatus.Empty;
            cellList = new();
            itemPositions.Add(jasonItem, new(0, 0));
            backpackRectTransform = backpack.GetComponent<RectTransform>();
            grid.OutputTXT();
            grid.ReverseColumns();
            grid.OutputTXT();
            grid.Transpose();
            grid.OutputTXT();

            //PlaceItem(testObject, new Vector2Int(0,2));
            //grid.OutputTXT();

            foreach (var x in grid)
            {
                Debug.Log(x.value + " " + x.row + " " + x.col);
            }

            InitialDrawInventoryToCanvas();
        }

        private void Update()
        {
            UpdateInventoryAlreadyOnCanvas();
        }

        //If marked dirty, we'll need to do a full refresh?
        void UpdateInventoryAlreadyOnCanvas()
        {
            var panelWidth = 1f / grid.NumColumns;
            var panelHeight = 1f / grid.NumRows;
            var panelOffsetX = backpackRectTransform.rect.height / grid.NumColumns;
            var panelOffsetY = backpackRectTransform.rect.height / grid.NumRows;
            var topLeftCorner = GetBackpackTopLeftCorner();

            foreach (var x in grid)
            {
                var panel = cellList[GetIndex(x.row, x.col)];
                panel.transform.localScale = new Vector3(panelWidth, panelHeight, 1);
                panel.transform.localPosition = GetNewPanelLocalPosition(topLeftCorner, panelOffsetX, panelOffsetY, x.row, x.col);
            }
        }

        public void InitialDrawInventoryToCanvas()
        {
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
            }
        }


        public bool PlaceItem(BaseItem item, Vector2Int desiredPos)
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
                    if (collGrid[i-desiredPos.x, j-desiredPos.y] != (int)Grid.CellStatus.Empty)
                    {
                        grid[i, j] = collGrid[i-desiredPos.x, j-desiredPos.y];
                    }
                }
            }
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