using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace PlayerInventory {

    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] Object panelPrefab;
        [SerializeField] Canvas canvas;
        [SerializeField] GameObject backpack;
        RectTransform backpackRectTransform;
        List<GameObject> panelList;
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
            grid = new(3, 4);
            panelList = new();
            backpackRectTransform = backpack.GetComponent<RectTransform>();
            //grid.OutputTXT();
            //grid.ReverseColumns();
            //grid.Transpose();
            //grid.OutputTXT();

            //PlaceItem(testObject, new Vector2Int(0,2));
            //grid.OutputTXT();

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
            for(int i = 0; i< grid.NumRows; i++)
            {
                for(int j = 0; j < grid.NumColumns; j++)
                {
                    var panel = panelList[GetIndex(i, j)];
                    panel.transform.localScale = new Vector3(panelWidth, panelHeight, 1);
                    panel.transform.localPosition = GetNewPanelLocalPosition(topLeftCorner, panelOffsetX, panelOffsetY, i, j);
                }
            }
        }


        public void InitialDrawInventoryToCanvas()
        {
            var panelWidth = 1f / grid.NumColumns;
            var panelHeight = 1f / grid.NumRows;

            var topLeftCorner = GetBackpackTopLeftCorner();

            var panelOffsetX = backpackRectTransform.rect.height / grid.NumColumns;
            var panelOffsetY = backpackRectTransform.rect.height / grid.NumRows;

            for (int i = 0; i < grid.NumRows; i++)
            {
                for (int j = 0; j < grid.NumColumns; j++)
                {
                    var panel = Instantiate(panelPrefab) as GameObject;
                    panel.transform.SetParent(backpack.transform, false);
                    panel.transform.localScale = new Vector3(panelWidth, panelHeight, 1);
                    panel.transform.localPosition = GetNewPanelLocalPosition(topLeftCorner, panelOffsetX, panelOffsetY, i, j);
                    panelList.Add(panel);
                }
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
    }

    [System.Serializable]

    //row first grid.
    public class Grid : IEnumerable
    {
        int numColumns;
        int numRows;
        public int[] matrix;

        public int NumColumns { get { return numColumns; } set { numColumns = value; } }
        public int NumRows { get { return numRows; } set { numRows = value; } }


        public enum CellStatus {
            Locked = -1,
            Empty = 0,
        }

        public Grid(int r, int c)
        {
            numRows = r;
            numColumns = c;
            matrix = new int[r * c];

            for (int i = 0; i < r; i++)
            {
                for (int j = 0; j < c; j++)
                {
                    if (j <= 0)
                    {
                        this[i,j] = 1;
                    }
                    else
                    {
                        this[i, j] = 0;
                    }
                }
            }
        }

        public int this[int r, int c]
        {
            get
            {
                return matrix[r * numColumns + c];
            }
            set
            {
                matrix[r*numColumns + c] = value;
            }
        }

        public void ReverseColumns()
        {
            for (int r = 0; r < numRows; r++)
            {
                for (int c = 0; c < numColumns / 2; c++)
                {
                    var tmp = this[r, c];
                    this[r, c] = this[r, numColumns - c - 1];
                    this[r, numColumns - c - 1] = tmp;
                }
            }
        }

        public void Transpose()
        {
            var newArray = new int[numColumns * numRows];
            for (int c = 0; c < numColumns; c++)
            {
                for (int r = 0; r < numRows; r++)
                {
                    newArray[c + r * numRows] = this[r, c];
                }
            }
            matrix = newArray;

            var tmp = numRows;
            numRows = numColumns;
            numColumns = tmp;
        }

        public void OutputTXT()
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path += "\\debug.txt";

            Debug.Log(path);
            StreamWriter writer = new(path, append: true);

            writer.Write('\n');
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numColumns; j++)
                {
                    writer.Write(this[i, j]);
                }
                writer.Write('\n');
            }
            writer.Close();
        }

        /// <summary>
        /// Compares two grids for collisions
        /// </summary>
        /// <param name="collGrid">The smaller grid to be checked against.</param>
        /// <param name="desiredPos">Where the top left corner of the grid will be placed.</param>
        /// <returns>If collision -> true; else -> false</returns>
        public bool CheckCollisionWithGrid(ref Grid collGrid, Vector2Int desiredPos)
        {

            // Will placing this grid on top of the other grid place the grid outside of the larger grid?
            // Or: Does it fit?
            if (collGrid.numRows + desiredPos.x > numRows || desiredPos.x < 0)
            {
                return true;
            }
            if (collGrid.numColumns + desiredPos.y > numRows || desiredPos.y < 0)
            {
                return true;
            }

            // Is there something else in the way currently?
            for (int i = desiredPos.x; i < collGrid.numRows + desiredPos.x; i++)
            {
                for (int j = desiredPos.y; j < collGrid.numColumns + desiredPos.y; j++)
                {
                    // Would the cell be need to become occupied and is it already occupied?
                    if (collGrid[i-desiredPos.x, j-desiredPos.y] != (int)CellStatus.Empty
                        && this[i, j] != (int)CellStatus.Empty)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        public IEnumerator GetEnumerator()
        {
            // implement a better one in the future and convert.
            return matrix.GetEnumerator();
        }
    }
}