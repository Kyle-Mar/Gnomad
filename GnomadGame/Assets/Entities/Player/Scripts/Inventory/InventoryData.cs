using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using Gnomad.Utils;

namespace Entities.Player.Inventory
{
    public class InventoryData
    {
        #region private members

        BaseItem jasonItem;

        Grid grid;
        GameObject? currentItem = null;
        Dictionary<BaseItem, Vector2Int> itemPositions = new();
        #endregion
        #region public fields
        public BaseItem JasonItem { get; private set; }
        public Grid Grid => grid;
        public GameObject? CurrentItem { get; private set; }
        public Dictionary<BaseItem, Vector2Int> ItemPositions => itemPositions;
        public int NumColumns => grid.NumColumns;
        public int NumRows => grid.NumRows;
        #endregion

        public int this[int i, int j]
        {
            get => grid[i, j];
            set => grid[i, j] = value;
        }


        public InventoryData SetJasonItem(BaseItem item)
        {
            this.jasonItem = item;
            return this;
        }

        public InventoryData(Grid grid)
        {
            this.grid = grid;
        }
        
        public bool PlaceItem(BaseItem item, Vector2Int desiredPos)
        {
            Grid collGrid = item.GetGrid();
            if (grid.CheckCollisionWithGrid(ref collGrid, desiredPos))
            {
                return false;
            }
            for (int r = desiredPos.y; r < collGrid.NumRows + desiredPos.y; r++)
            {
                for (int c = desiredPos.x; c < collGrid.NumColumns + desiredPos.x; c++)
                {
                    if (collGrid[r - desiredPos.y, c - desiredPos.x] != (int)Grid.CellStatus.Empty)
                    {
                        grid[r, c] = collGrid[r - desiredPos.y, c - desiredPos.x];
                    }
                }
            }
            itemPositions.Add(item, desiredPos);
            return true;
        }

        public InventoryData SetCurrentItem(GameObject? item)
        {
            this.CurrentItem = item;
            return this;
        }

        public bool TryRemoveItem(BaseItem item, Vector2Int desiredPos)
        {
            Grid collGrid = item.GetGrid();
            //grid.OutputTXT();
            //collGrid.OutputTXT();
            for (int r = desiredPos.y; r < collGrid.NumRows + desiredPos.y; r++)
            {
                for (int c = desiredPos.x; c < collGrid.NumColumns + desiredPos.x; c++)
                {
                    Debug.Log("REMOVE: " + r + " " + c);
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

    }
}
