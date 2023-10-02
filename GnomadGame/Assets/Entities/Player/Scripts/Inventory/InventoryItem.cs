using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerInventory
{
    public class InventoryItem : MonoBehaviour
    {
        [SerializeField] private BaseItem item;
        public BaseItem Item => item;

        public void Initialize(BaseItem item)
        {
            this.item = item;
        }
    }
}
