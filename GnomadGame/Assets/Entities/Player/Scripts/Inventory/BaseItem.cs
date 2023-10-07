using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Entities.Player.Inventory{

    

    [CreateAssetMenu(menuName = "Items/Create New Item", order = 1)]
    public class BaseItem : ScriptableObject
    {
        // The grid that represents where the item is and is not.
        public Grid grid = new(1,1,0);
        public Texture2D itemTexture;
        //unset value flag but not nullable because that's weird with 0's
        public int ItemID = -0xBEEF;
        
        //Image at somepoint

        private void Awake()
        {
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

#if UNITY_EDITOR
    [CustomEditor(typeof(BaseItem))]
    [CanEditMultipleObjects]
    public class ItemEditor: UnityEditor.Editor
    {
        SerializedProperty grid;
        List<int> values;
        GUIStyle guiStyle;

        private void OnEnable()
        {
            var item = target as BaseItem;
            guiStyle = new();
            guiStyle.fixedWidth = 10f;
            values = new(item.grid.matrix.Length);
            for (int i = 0; i < item.grid.matrix.Length; i++)
            {
                values.Add(item.grid.matrix[i]);
            }
            grid = serializedObject.FindProperty("grid");
            
        }
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var item = target as BaseItem;

            // Labels and Floats for the properties of the grid.
            EditorGUILayout.LabelField("NumColumns");
            EditorGUILayout.FloatField(item.grid.NumColumns);
            EditorGUILayout.LabelField("NumRows");
            EditorGUILayout.FloatField(item.grid.NumRows);
            
            // if the row button is clicked
            if (GUILayout.Button("Add Row"))
            {
                // add a row, make a new grid and set the grid to the new grid.
                for(int i = 0; i < item.grid.NumColumns; i++)
                {
                    values.Add(0);
                }
                item.grid.NumRows++;
                int[] arr = new int[ item.grid.NumRows * item.grid.NumColumns];
                System.Array.Copy(item.grid.matrix, arr, item.grid.matrix.Length);
                item.grid.matrix = arr;
            }
            if (GUILayout.Button("Remove Row"))
            {
                // remove a row, make a new grid and set the grid to the new grid.
                for (int i = 0; i < item.grid.NumColumns; i++)
                {
                    values.RemoveAt(values.Count - 1);
                }
                item.grid.NumRows--;
                int[] arr = new int[item.grid.NumRows * item.grid.NumColumns];
                System.Array.Copy(item.grid.matrix, arr, (item.grid.NumRows) * item.grid.NumColumns);
                item.grid.matrix = arr;
            }
            if (GUILayout.Button("Add Column"))
            {
                // add a column, make a new grid and set the grid to the new grid.
                for (int i = 0; i < item.grid.NumRows; i++)
                {
                    values.Add(0);
                }
                item.grid.NumColumns++;
                int[] arr = new int[item.grid.NumRows * item.grid.NumColumns];
                System.Array.Copy(item.grid.matrix, arr, item.grid.matrix.Length);
                item.grid.matrix = arr;
            }
            if (GUILayout.Button("Remove Column"))
            {
                // remove a column, make a new grid and set the grid to the new grid.
                for (int i = 0; i < item.grid.NumRows; i++)
                {
                    values.RemoveAt(values.Count -1);
                }
                item.grid.NumColumns--;
                int[] arr = new int[item.grid.NumRows * item.grid.NumColumns];
                System.Array.Copy(item.grid.matrix, arr, (item.grid.NumRows) * item.grid.NumColumns);
                item.grid.matrix = arr;
            }

            // update the properties of the object.
            // this updates the properties of the BaseItem we're editing.
            serializedObject.Update();

            // draw the rows and columns of the grid.
            EditorGUILayout.BeginHorizontal();
            for (int i = 0, j = 0; i < values.Count; i++, j++)
            {
                // new line if we're exceeding numcolumns
                if (j >= item.grid.NumColumns)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    j = 0;
                }
                // then make a field for each number and set the displayed value to the grid value.
                var num = EditorGUILayout.IntField(item.grid.matrix[i], GUILayout.Width(50f));
                // make sure the value is either 0 or the item id, anything else should be considered invalid and set to the item id.
                if(num == item.ItemID)
                {
                    values[i] = item.ItemID;
                }
                else if(num == 0)
                {
                    values[i] = 0; 
                }
                else if(num != item.ItemID)
                {
                    values[i] = item.ItemID;
                }
            }
            EditorGUILayout.EndHorizontal();
            // set the values.
            for(int i = 0; i<item.grid.matrix.Length; i++)
            {
                item.grid.matrix[i] = values[i];
            }
            // update Baseitem
            serializedObject.Update();

        }
    }
    #endif
}

