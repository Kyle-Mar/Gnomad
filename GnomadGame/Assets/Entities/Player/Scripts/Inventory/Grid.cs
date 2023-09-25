using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;


namespace PlayerInventory
{

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
        public int[] matrix;

        public int NumColumns { get { return numColumns; } set { numColumns = value; } }
        public int NumRows { get { return numRows; } set { numRows = value; } }


        public enum CellStatus
        {
            Locked = -1,
            Empty = 0,
        }

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
                    if (collGrid[i - desiredPos.x, j - desiredPos.y] != (int)CellStatus.Empty
                        && this[i, j] != (int)CellStatus.Empty)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public IEnumerator<GridTuple> GetEnumerator()
        {
            return new GridEnumerator(this.matrix, this.numColumns, this.numRows);
        }

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


            public GridEnumerator(int[] matrix, int numColumns, int numRows)
            {
                this.matrix = matrix;
                this.numColumns = numColumns;
                this.numRows = numRows;
            }

            public void Dispose()
            {

            }

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

            public void Reset()
            {
                this.curCol = 0;
                this.curRow = 0;
                this.idx = 0;
            }
        }
    }
}