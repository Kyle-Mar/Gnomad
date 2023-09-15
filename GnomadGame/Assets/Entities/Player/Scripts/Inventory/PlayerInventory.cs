using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace PlayerInventory {

    public class PlayerInventory : MonoBehaviour
    {
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
            //grid.OutputTXT();
            //grid.ReverseColumns();
            //grid.Transpose();
            //grid.OutputTXT();

            //PlaceItem(testObject, new Vector2Int(0,2));
            grid.OutputTXT();

            
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
    }

    [System.Serializable]
    public class Grid : IEnumerable
    {
        int numColumns;
        int numRows;
        public int[] matrix;

        public int NumColumns { get { return numColumns; } set { numColumns = value; } }
        public int NumRows { get { return numRows; } set { numRows = value; } }


        public enum CellStatus {
            Empty,
            Occupied
        }

        public Grid(int r, int c)
        {
            numRows = r;
            numColumns = c;
            matrix = new int[r*c];

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
            get => matrix[r + c*numColumns];
            set
            {
                matrix[r+c*numColumns] = value;
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