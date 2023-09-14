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


        private void Awake()
        {
            var testObject = gameObject.AddComponent<TestItem>().GetGrid();
            grid = new(3, 4);
            //grid.OutputTXT();
            //grid.ReverseColumns();
            //grid.Transpose();
            //grid.OutputTXT();

            Debug.Log(grid.CheckCollisionWithGrid(ref testObject, new Vector2Int(0, 0)));
        }
    }

    public class Grid : IEnumerable
    {
        int numColumns;
        int numRows;
        int[,] matrix;

        public enum CellStatus {
            Empty,
            Occupied
        }

        public Grid(int r, int c)
        {
            numRows = r;
            numColumns = c;
            matrix = new int[r, c];
            for (int i = 0; i < r; i++)
            {
                for (int j = 0; j < c; j++)
                {
                    if (j <= 0)
                    {
                        matrix[i, j] = 1;
                    }
                    else
                    {
                        matrix[i, j] = 0;
                    }
                }
            }
        }

        public int this[int r, int c]
        {
            get => matrix[r, c];
            set
            {
                matrix[r, c] = value;
            }
        }

        public void ReverseColumns()
        {
            for (int r = 0; r < numRows; r++)
            {
                for (int c = 0; c < numColumns / 2; c++)
                {
                    var tmp = matrix[r, c];
                    matrix[r, c] = matrix[r, numColumns - c - 1];
                    matrix[r, numColumns - c - 1] = tmp;
                }
            }
        }

        public void Transpose()
        {
            var newArray = new int[numColumns, numRows];
            for (int c = 0; c < numColumns; c++)
            {
                for (int r = 0; r < numRows; r++)
                {
                    newArray[c, r] = matrix[r, c];
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
                    writer.Write(matrix[i, j]);
                }
                writer.Write('\n');
            }
            writer.Close();
        }

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
                    if (collGrid[i, j] != (int)CellStatus.Empty
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