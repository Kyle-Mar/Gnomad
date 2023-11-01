using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditor.UI;
using Unity.VisualScripting;
using System.Drawing.Printing;
using System.Linq;
using System.Collections.Concurrent;
using UnityEngine.UIElements;

public class EnvironmentEditor : EditorWindow
{
    #region Variables
    //Prefab Info
    private static string prefabFolderPath = "Assets/Zones/ForestZone/Environmental Assets/Elements/EnvironmentEditorPool";
    //private List<GameObject> prefabList = new List<GameObject>();
    List<List<GameObject>> SubfolderPrefabLists = new List<List<GameObject>>();
    //Window Info
    private Vector2 scrollPosition;
    //Painting Variables
    private GameObject selectedBrush;
    private GameObject lastBrush;
    private GameObject lastPlacedPref;
    private Stack<GameObject> prefabHistory;
    bool paintBehind = true;
    private bool rotateMode;
    //Layer Info
    private int currentLayer = 4;
    private GameObject[] layersList;
    //GUI Variables
    private int prefabButtonSize = 64;
    GUIContent currentLayerIcon;

    //----Variable Interfaces----
    public int CurrentLayer
    {
        get { return currentLayer; }
        set
        {
            currentLayer = Mathf.Clamp(value, 0, layersList.Length - 1);
            EditorUtility.SetDefaultParentObject(layersList[currentLayer]);
        }
    }

    #endregion variables

    [MenuItem("Window/Environment Editor")]

    #region Initialization
    static void Init()
    {   //Runs once when the Editor is first Opened. Initializes the window
        //This needs to run at a different stage in settup from OnEnable, so it needs to be kept seperate
        Debug.Log("Initializing EDITOR");
        EnvironmentEditor window = GetWindow<EnvironmentEditor>();
        window.minSize = new Vector2(200, 200);
        window.Show();
    }

    private void OnEnable()
    {
        //Also runs once on startup
        Debug.Log("Enabling EDITOR");
        //GetPrefabs();
        GetPrefabsInSubfolders();
        initializeLayers();
  
        SceneView.duringSceneGui += OnSceneGUI;
        rotateMode = false;
        paintBehind = true;
        prefabHistory = new Stack<GameObject>();
    }
    #endregion Initialization

    private void OnGUI()
    {   
        if (layersList.Length < 1) {
            GUILayout.Label("Not a valid environment heirarchy.\n Make sure your layers are properly tagged\n and you are using the level template.");
            if (GUILayout.Button("Refresh"))
            {
                OnEnable();
                EditorApplication.RepaintHierarchyWindow();
            }
            return; 
        }

        GUILayout.Label(
            "Click a prop icon below to set the BRUSH\n" +
            "Hold 'E' to ROTATE the last painted prop\n" +
            "Press Ctrl+Z to DESTROY the last painted prop\n" +
            "Press 'Z' To clear the current brush and leave edit mode\n" +
            "Press 'X' to use the last brush\n" +
            "Press '-' and '+' to go back and forth between layers\n" +
            "Press '[' or ']' too change order within layer\n" + 
            "Press 'L' I think to flip the sprite"
            );
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        //ToDo
        //
        // alternative paint modes
        //change prop size
        //  on ctrl + or ctrl - clicked
        //      check neighbor in prefab array for same name with different last letter
        //add icon for current and last brushes
        //dont draw when clicking to move a selected object
        //add heirarchy for brush types
        //after being placed, props should scoot until mouse up

        drawStateIcons();
        foreach (List<GameObject> l in SubfolderPrefabLists)
        {
            DrawPrefabIconGrid(l, prefabButtonSize, (int)position.width / prefabButtonSize);
        }

        EditorGUILayout.EndScrollView();
        Event e = Event.current;


        if (GUILayout.Button("Clear Brush"))
        {
            ClearBrush();
        }
        checkInputCommon(e);
    }

    private void DrawPrefabIconGrid(List<GameObject> prefabList, int buttonSize, int itemsPerRow)
    {
        GUILayout.BeginVertical();

        for (int i = 0; i < prefabList.Count; i++)
        {
            if (i % itemsPerRow == 0)
            {
                GUILayout.BeginHorizontal();
            }

            GameObject prefab = prefabList[i];

            if (prefab != null)
            {
                GUIContent content = new GUIContent();
                Texture2D icon = AssetPreview.GetAssetPreview(prefab);

                if (icon != null)
                {
                    content.image = icon;
                    content.tooltip = prefab.name;
                }

                if (GUILayout.Button(content, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
                {
                    lastBrush = selectedBrush;
                    selectedBrush = prefab;
                    Debug.Log("Selected prefab is " + prefab.name);
                }
            }

            if ((i + 1) % itemsPerRow == 0 || i == prefabList.Count - 1)
            {
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndVertical();
    }

    private void ClearBrush()
    {
        selectedBrush = null;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (layersList.Length < 1) { return; }
        Event e = Event.current;
        //stamp
        if (e.type == EventType.MouseDown && e.button == 0 && selectedBrush != null)
        {
            lastPlacedPref = PrefabUtility.InstantiatePrefab(selectedBrush) as GameObject;
            lastPlacedPref.transform.position = GetMousePosition();
            lastPlacedPref.transform.parent = layersList[CurrentLayer].transform.GetChild(0);
            if (paintBehind)
            {
                lastPlacedPref.transform.SetAsFirstSibling();
            }
            if (prefabHistory.Count > 40)
            {//remove first item
                prefabHistory.Reverse();
                prefabHistory.Pop();
                prefabHistory.Reverse();
            }
            prefabHistory.Push(lastPlacedPref);



        }
        UpdateEventStates(e);
        //rotate
        if (rotateMode == true && lastPlacedPref != null)
        {
            Vector3 mousePosition = GetMousePosition();
            Vector3 objectPosition;
            if (Selection.activeTransform)
            {
                objectPosition = Selection.activeTransform.position;
            }
            else
            {
                objectPosition = lastPlacedPref.transform.position;
            }

            // Calculate the angle in radians between the object and the mouse position
            float angle = Mathf.Atan2(mousePosition.y - objectPosition.y, mousePosition.x - objectPosition.x);

            // Convert the angle to degrees and create a rotation quaternion around the z-axis
            Quaternion rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
            if (Selection.activeTransform == null)
            {
                lastPlacedPref.transform.rotation = rotation;
            }
            else { Selection.activeTransform.rotation = rotation; }

        }

        if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.Z)
        {
            if (lastPlacedPref != null)
            {
                DestroyImmediate(prefabHistory.Pop());
                lastPlacedPref = null;
                if (prefabHistory.Count > 0)
                {
                    lastPlacedPref = prefabHistory.Peek();
                }
            }
            e.Use();
        }

        checkInputCommon(e);



    }

    /*
     private void GetPrefabs()
     {
         prefabList.Clear();

         string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[] { prefabFolderPath });
         foreach (string guid in guids)
         {
             string path = AssetDatabase.GUIDToAssetPath(guid);
             GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
             if (prefab != null)
             {
                 prefabList.Add(prefab);

             }
         }
     }*/
    private void GetPrefabsInSubfolders()
    {

        string[] subfolderPaths = AssetDatabase.GetSubFolders(prefabFolderPath);

        foreach (string subfolderPath in subfolderPaths)
        {
            List<GameObject> subfolderPrefabList = new List<GameObject>();
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[] { subfolderPath });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null)
                {
                    subfolderPrefabList.Add(prefab);
                }
            }
            SubfolderPrefabLists.Add(subfolderPrefabList);
        }

    }

    public Vector3 GetMousePosition()
    {
        Event e = Event.current;

        Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        float z = 0; // Set z to 0 for 2D games
        return new Vector3(worldRay.origin.x, worldRay.origin.y, z);

    }

    private void UpdateEventStates(Event e)
    {
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.E) { rotateMode = true; }
        else if (e.type == EventType.KeyUp && e.keyCode == KeyCode.E) { rotateMode = false; }

    }

    private int initializeLayers()
    {   //might not work. Array is immuatable
        layersList = GameObject.FindGameObjectsWithTag("ParalaxLayer");
        if (layersList == null) { return -1; }
        return 0;
    }

    private void checkInputCommon(Event e)
    {//some input actions need to checked from both focuses
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Minus)
        {
            CurrentLayer = CurrentLayer - 1;
            Debug.Log(layersList[CurrentLayer].name);
            foreach (GameObject l in layersList)
            {
                if (l == layersList[currentLayer]) { l.hideFlags = HideFlags.None; }
                l.hideFlags = HideFlags.NotEditable;
            }
            EditorApplication.RepaintHierarchyWindow();
            drawStateIcons();

        }
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Equals)
        {
            CurrentLayer = CurrentLayer + 1;
            Debug.Log(layersList[CurrentLayer].name);
            foreach (GameObject l in layersList)
            {
                if (l == layersList[currentLayer]) { l.hideFlags = HideFlags.None; }
                l.hideFlags = HideFlags.NotEditable;
            }
            EditorApplication.RepaintHierarchyWindow();
            drawStateIcons();
        }

        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Z)
        {
            ClearBrush();
        }
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.LeftBracket)
        {
            if (Selection.activeTransform != null)
            {
                Selection.activeTransform.SetSiblingIndex(Selection.activeTransform.GetSiblingIndex() - 1);
            }
            else
            {
                lastPlacedPref.transform.SetSiblingIndex(lastPlacedPref.transform.GetSiblingIndex() - 1);
            }
        }
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.RightBracket)
        {
            if (Selection.activeTransform != null)
            {
                Selection.activeTransform.SetSiblingIndex(Selection.activeTransform.GetSiblingIndex() + 1);
            }
            else
            {
                lastPlacedPref.transform.SetSiblingIndex(lastPlacedPref.transform.GetSiblingIndex() + 1);
            }
        }
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.X)
        {
            GameObject tmp = selectedBrush;
            selectedBrush = lastBrush;
            lastBrush = tmp;
        }
        if(e.type == EventType.KeyDown && e.keyCode == KeyCode.L)
        {
            if (Selection.activeTransform != null)
            {
                Selection.activeTransform.GetComponent<SpriteRenderer>().flipX = !Selection.activeTransform.GetComponent<SpriteRenderer>().flipX;
            }
            else
            {
                lastPlacedPref.transform.GetComponent<SpriteRenderer>().flipX = !lastPlacedPref.transform.GetComponent<SpriteRenderer>().flipX;
            }
        }
        
    }

    //draws icons for current brush, current layer, etc
    private void drawStateIcons()
    {
        int iconBoxWidth = 128;
        int iconBoxHeight = 240;
        GUILayout.BeginArea(new Rect(position.width - iconBoxWidth, 0, position.width, iconBoxHeight)); // Adjust the height (20) as needed

        // Draw a label with the text icon
        currentLayerIcon = new GUIContent("Current Layer\n" + layersList[CurrentLayer].name, "Selected paralax layer");
        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.white;

        GUILayout.Label(currentLayerIcon, style);

        GUILayout.EndArea();
        Repaint();
    }


}