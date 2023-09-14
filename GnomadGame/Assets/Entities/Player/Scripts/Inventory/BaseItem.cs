using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PlayerInventory{
    public abstract class BaseItem : MonoBehaviour
    {
        Grid grid;
        //Image at somepoint

        private void Awake()
        {
            grid = new Grid(1,1);
            grid[0, 0] = 1;
            //grid[0, 1] = 1;
            grid.OutputTXT();

        }

        public virtual void ApplyEffect()
        {
            Debug.Log("Base Effect");
        }

        public Grid GetGrid()
        {
            return grid;
        }
    }

    public class TestItem : BaseItem
    {
        
    }
}

