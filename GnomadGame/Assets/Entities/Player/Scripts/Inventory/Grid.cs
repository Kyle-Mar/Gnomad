using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;


namespace Entities.Player.Inventory
{

    /// <summary>
    /// return type for Enumerables (used in foreach statements)
    /// </summary>
    public struct GridTuple
    {
        public int value;
        public int row;
        public int col;
    }
    //row first grid.
    [System.Serializable]
    public class Grid : IEnumerable<GridTuple>
    {
        int numColumns;
        int numRows;
        // a flattened array because 2d arrays are hard to serialize.
        public int[] matrix;

        // at some point it may be worth it to write bodies in here to automatically rebuild the grid when the value is changed. For right now, we don't do that.
        public int NumColumns { get { return numColumns; } set { numColumns = value; } }
        public int NumRows { get { return numRows; } set { numRows = value; } }

        /// <summary>
        /// Determines the status of the InventoryCell. Cells may also be > 1 if they are occupied by an item instead.
        /// </summary>
        public enum CellStatus
        {
            Locked = -1,
            Empty = 0,
        }
        
        /// <summary>
        /// Initializes a Grid with r by c cells
        /// </summary>
        /// <param name="r">number of rows</param>
        /// <param name="c">number of columns</param>
        /// <param name="defaultValue">the value that the grid should be initialized to</param>
        public Grid(int r, int c, int defaultValue)
        {
            numRows = r;
            numColumns = c;
            matrix = new int[r * c];

            foreach (var x in this)
            {
                this[x.row,x.col] = defaultValue;
            }
        }

        /// <summary>
        /// to do use Grid[r,c] and return the value from the matrix flattened array
        /// </summary>
        /// <param name="r">Row Number</param>
        /// <param name="c">Column Number</param>
        /// <returns></returns>
        public int this[int r, int c]
        {
            get
            {
                return matrix[r * numColumns + c];
            }
            set
            {
                matrix[r * numColumns + c] = value;
            }
        }

        /// <summary>
        /// Reverses the columns in the grid. Cannot be converted to IEnumerable because we have to go halfway throught the array only
        /// </summary>
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

        /// <summary>
        /// Transposes the grid as if it were a matrix.
        /// </summary>
        public void Transpose()
        {
            var newArray = new int[numColumns * numRows];

            for (int c = 0; c < numColumns ; c++)
            {
                for (int r = 0; r < numRows; r++)
                {
                    newArray[newArray.Length - 1 - (c * numRows + r)] = this[r,c];
                }
            }
            matrix = newArray;

            var tmp = numRows;
            numRows = numColumns;
            numColumns = tmp;
        }

        /// <summary>
        /// Outputs a txt file to the ScriptAssemblies folder (Not Ideal)
        /// </summary>
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

        public bool IsInvalidCell(Grid grid, int r, int c)
        {
             Debug.Log(grid[r, c]);
             return (grid[r, c] != (int)CellStatus.Empty);
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
            if (collGrid.numColumns + desiredPos.x > NumColumns || desiredPos.x < 0)
            {
                Debug.Log("too big columns");
                return true;
            }
            if (collGrid.numRows + desiredPos.y > numRows || desiredPos.y < 0)
            {
                Debug.Log("too big rows");
                return true;
            }

            // Is there something else in the way currently?

            for (int r = desiredPos.y; r < collGrid.NumRows + desiredPos.y; r++)
            {
                for (int c = desiredPos.x; c < collGrid.numColumns + desiredPos.x; c++)
                {
                    // Would the cell be need to become occupied and is it already occupied?
                    if (IsInvalidCell(collGrid, r - desiredPos.y, c - desiredPos.x) && IsInvalidCell(this, r, c))
                    {
                        Debug.Log("something in the way.");
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Used for foreach loops
        /// </summary>
        /// <returns>Grid IEnumerator</returns>
        public IEnumerator<GridTuple> GetEnumerator()
        {
            return new GridEnumerator(this.matrix, this.numColumns, this.numRows);
        }

        /// <summary>
        /// generic thing idk what its used for.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new GridEnumerator(this.matrix, this.numColumns, this.numRows);
        }

        public class GridEnumerator : IEnumerator<GridTuple>
        {
            public GridTuple Current = new GridTuple { value = 0, row = 0, col = 0 };
            private int curRow = 0;
            private int curCol = 0;
            private int numColumns;
            private int numRows;
            private int idx = 0;
            int[] matrix;
            object IEnumerator.Current => Current;
            GridTuple IEnumerator<GridTuple>.Current => Current;


            // initializes the values with the grid's values
            public GridEnumerator(int[] matrix, int numColumns, int numRows)
            {
                this.matrix = matrix;
                this.numColumns = numColumns;
                this.numRows = numRows;
            }

            public void Dispose()
            {

            }

            // goes to the next item in the grid.
            public bool MoveNext()
            {
                if (idx > matrix.Length - 1)
                {
                    return false;
                }
                curCol = idx % numColumns;
                curRow = idx / numColumns;
                Current = new GridTuple { value = matrix[idx], row = curRow, col = curCol };
                idx++;
                return true;
            }

            // resets the values
            public void Reset()
            {
                this.curCol = 0;
                this.curRow = 0;
                this.idx = 0;
            }
        }
    }
}