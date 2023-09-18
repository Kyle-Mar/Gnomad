using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace PlayerInventory{

    

    [CreateAssetMenu(menuName = "Items/Create New Item", order = 1)]
    public class BaseItem : ScriptableObject
    {
        
        public Grid grid = new(1,1);
        //unset value flag but not nullable because that's weird with 0's
        public int ItemID = -1;
        
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
            EditorGUILayout.PropertyField(grid);
            var item = target as BaseItem;
            EditorGUILayout.LabelField("NumColumns");
            EditorGUILayout.FloatField(item.grid.NumColumns);
            EditorGUILayout.LabelField("NumRows");
            EditorGUILayout.FloatField(item.grid.NumRows);
            if (GUILayout.Button("Add Row"))
            {
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
                for (int i = 0; i < item.grid.NumRows; i++)
                {
                    values.RemoveAt(values.Count -1);
                }
                item.grid.NumColumns--;
                int[] arr = new int[item.grid.NumRows * item.grid.NumColumns];
                System.Array.Copy(item.grid.matrix, arr, (item.grid.NumRows) * item.grid.NumColumns);
                item.grid.matrix = arr;
            }
            serializedObject.Update();


            EditorGUILayout.BeginHorizontal();
            for (int i = 0, j = 0; i < values.Count; i++, j++)
            {
                if (j >= item.grid.NumColumns)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    j = 0;
                }
                var num = EditorGUILayout.IntField(item.grid.matrix[i], GUILayout.Width(50f)); ;
            }
            EditorGUILayout.EndHorizontal();
            for(int i = 0; i<item.grid.matrix.Length; i++)
            {
                item.grid.matrix[i] = values[i];
            }
            serializedObject.Update();

        }
    }


    [CustomEditor(typeof(Grid))]
    public class GridEditor : UnityEditor.Editor
    {
        SerializedProperty matrix;
        private void OnEnable()
        {
            // link the property.
            matrix = serializedObject.FindProperty("matrix");
        }

        public override void OnInspectorGUI()
        {
            Debug.Log(matrix.type);
            DrawDefaultInspector();
            
            //Load the values from the object.
            serializedObject.Update();

            for(int i =0; i < matrix.arraySize; i++)
            {
                EditorGUILayout.IntField(matrix.GetArrayElementAtIndex(i).intValue);
            }
            serializedObject.ApplyModifiedProperties();
        }

    }
    #endif
}

