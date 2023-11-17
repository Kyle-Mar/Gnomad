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
using UnityEngine.EventSystems;
using UnityEditor.SceneManagement;
using UnityEditor.TerrainTools;
using PlasticGui.WorkspaceWindow.Home.Workspaces;
using static PlasticGui.WorkspaceWindow.Items.ExpandedTreeNode;
using SandolkakosDigital.EditorUtils;
using static Codice.CM.WorkspaceServer.WorkspaceTreeDataStore;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using PlasticPipe.PlasticProtocol.Client;
#if UNITY_EDITOR
//TODO
/*
Create different tabs in the folder view zone
rotate/scale/flip indicators
Move objects on paint and drag
implement rotation mode
set last painted prefab to selected prefab
automatically set render layer of placed props
Implement light tab
Implement actor tab
Implement collection tab
Implement hotkeys popup button
fix ctrl-z discrepency and rectiy distinction between selected and last placed prefab
toggle to change tileset and sky to bright colors so holes can be seen
*/
public class EnvironmentEditor : EditorWindow
{
    #region Variables
    //Prefab Info
    private static string prefabFolderPath = "Assets/Zones/ForestZone/Environmental Assets/Elements/EnvironmentEditorPool";
    //private List<GameObject> prefabList = new List<GameObject>();
    public class StampFolder
    {
        public List<GameObject> Stamps;
        public String Name;
        public bool Active;

        public StampFolder(string name, List<GameObject> stamps, bool active)
        {
            Name = name;
            Stamps = stamps;
            Active = active;
        }

    }
    //List<Tuple<String, List<GameObject>>> SubfolderPrefabLists = new List<Tuple<String, List<GameObject>>>();
    //List<bool> prefabCategoryActivationStatuses = new List<bool>();
    List<StampFolder> stampFolders = new List<StampFolder>();
    //Window Info
    private Vector2 scrollPosition;
    //Painting Variables
    private GameObject selectedStamp;
    private GameObject lastBrush;
    private GameObject lastPlacedPref;
    private Stack<GameObject> prefabHistory;
    private BrushData CurrentBrush;
    public enum PaintMode
    {
        PaintBehind,
        PaintMiddle,
        PaintInFront
    }
    private PaintMode paintMode;

    //Layer Info
    private int currentLayer = 4;
    private GameObject[] layersList;
    private bool hideMode = false;
    //GUI Variables
    private int prefabButtonSize = 64;
    GUIContent currentLayerIcon;
    //Textures
    private Texture2D layerButtonsTexture;
    private Color layerButtonsColor = new Color(0.01f, 0.01f, 1f, 1f);
    //Event/State Information
    bool mouseHeldRight = false;
    bool mouseHeldLeft = false;
    private bool rotateMode;
    private bool ScootMode;
    //this is used to select the last placed object
    //something after us selects a different object sometimes, so we reselect next frame
    private bool placedObjectLastFrame; 

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

    [MenuItem("Window/Environment Editor/Environment Editor")]

    #region Initialization
    /// <summary>
    /// creates the window. Must be created before OnEnable()
    /// </summary>
    static void Init()
    {   //Runs once when the Editor is first Opened. Initializes the window
        //This needs to run at a different stage in settup from OnEnable, so it needs to be kept seperate
        Debug.Log("Initializing EDITOR");
        EnvironmentEditor window = GetWindow<EnvironmentEditor>();
        window.minSize = new Vector2(600, 600);
        window.Show();
    }

    /// <summary>
    /// Initialize assets from folders. Once on start up
    /// </summary>
    private void OnEnable()
    {
        PopulatePrefabList();
        initializeLayers();

        SceneView.duringSceneGui += OnSceneGUI;

        rotateMode = false;
        paintMode = PaintMode.PaintBehind;
        prefabHistory = new Stack<GameObject>();
    }


    private void PopulatePrefabList()
    {
        string[] subfolderPaths = AssetDatabase.GetSubFolders(prefabFolderPath);

        foreach (string subfolderPath in subfolderPaths)
        {
            string[] subSubfolderPaths = AssetDatabase.GetSubFolders(subfolderPath);
            List<GameObject> subfolderPrefabList = new List<GameObject>();
            foreach (string subSubfolderPath in subSubfolderPaths)
            {
                string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[] { subSubfolderPath });
                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (prefab != null)
                    {
                        subfolderPrefabList.Add(prefab);
                        //prefabCategoryActivationStatuses.Add(false);
                    }
                }
            }
            if (subfolderPrefabList.Count > 0)
            {
                string topLevelFolderName = Path.GetFileName(subfolderPath);
                //Tuple<string, List<GameObject>> topLevelFolderPrefabTuple = new Tuple<string, List<GameObject>>(topLevelFolderName, subfolderPrefabList);
                //SubfolderPrefabLists.Add(topLevelFolderPrefabTuple);
                //Debug.Log(topLevelFolderPrefabTuple);
                stampFolders.Add(new StampFolder(topLevelFolderName, subfolderPrefabList, false));

            }
        }
    }

    private int initializeLayers()
    {   //might not work. Array is immuatable
        layersList = GameObject.FindGameObjectsWithTag("ParalaxLayer");
        if (layersList == null) { return -1; }
        return 0;
    }
    #endregion Initialization

    #region Update

    /// <summary>
    /// called when GUI is updated or moused over
    /// </summary>
    private void OnGUI()
    {
        Event e = Event.current;
        HandleInput(e);
        if (layersList.Length < 1)//make sure we are in a valid room
        {
            GUILayout.Label("Not a valid environment heirarchy.\n Make sure your layers are properly tagged\n and you are using the level template.");
            if (GUILayout.Button("Refresh"))
            {
                OnEnable();
                EditorApplication.RepaintHierarchyWindow();
            }
            return;
        }


        DrawLayerButtons();
        GUILayout.BeginArea(new Rect(position.width / 3 + 10,
             10,
             position.width / 3 + 10,
             position.height / 2)
             );
        DrawBrushHotkeys();
        GUILayout.EndArea();
        DrawBrushButtons();

        DrawGrids();
    }
    private void OnSceneGUI(SceneView sceneView)
    {
        if (layersList.Length < 1) { return; }
        if (placedObjectLastFrame) {
            Selection.activeObject = lastPlacedPref; 
            placedObjectLastFrame = false;
        }
        Event e = Event.current;
        //if (e.button == 0 && e.isMouse)
        HandleInput(e);

        if (e.type == EventType.MouseUp)
        {
            mouseHeldLeft = false;
            ScootMode = false;
            //Debug.Log("Mouse NOT Held Left!");
        }

        if (e.type == EventType.MouseDown && e.button == 0 && selectedStamp != null)
        {
            Paint();
            e.Use();
        }

    }
    private void Update()
    {
        if (ScootMode)
        {
            if (lastPlacedPref == null) {
                Debug.Log("Scoot mode pref is null");
                ScootMode = false;
                return;
            }
            if (!mouseHeldLeft)
            {
                Debug.Log("Scoot mode mouse not held");
                ScootMode = false;
                return;
            }
            lastPlacedPref.transform.position = new Vector3(GetMousePosition().x, GetMousePosition().y, 0);
        }
    }
    private void HandleInput(Event e)
    {/*
        if (e.isMouse && e.button == 0)
        {
            mouseHeldLeft = true;
            Debug.Log("Mouse Held Left!");
        }
        if (e.button != 0)
        {
            mouseHeldLeft = false;
            Debug.Log("Mouse NOT Held Left!");
        }*/
        if (e.type == EventType.KeyDown)
        {
            HandleKeyDownInput(e);
        }

    }

    /// <summary>
    /// Handles simple hotkeys
    /// </summary>
    /// <param name="e"></param>
    private void HandleKeyDownInput(Event e)
    {
        switch (e.keyCode)
        {
            case KeyCode.Minus:
                OnCLickLayerDown();
                break;
            case KeyCode.Equals:
                OnCLickLayerUp();
                break;
            case KeyCode.LeftBracket:
                if (e.alt) { OnCLickPaintBehind(); }
                else { MoveBackInLayer(); }
                break;
            case KeyCode.Backslash:
                if (e.alt) { OnCLickPaintMiddle(); }
                else { MoveMiddleInLayer(); }
                break;
            case KeyCode.RightBracket:
                if (e.alt) { OnCLickPaintInFront(); }
                else { MoveForwardInLayer(); }
                break;
            case KeyCode.H:
                OnCLickIsolateLayer();
                break;
            case KeyCode.L:
                if (e.alt) { FlipPrefabY(); }
                else { FlipPrefabX(); }
                break;
            case KeyCode.X:
                UseLastBrush();
                break;
            case KeyCode.Z:
                if (e.control) { OnCLickUndo(); }
                else { ClearStamp(); }
                break;

        }
    }

    #endregion Update

    #region DrawingGUI
    private void DrawLayerButtons()
    {
        GUILayout.BeginArea(new Rect(position.width - position.width / 4,
     10,
     position.width / 4,
     position.height / 2)
     );

        // Create a group of vertical buttons
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        // "Current Layer" button with "Up" and "Down" buttons arranged vertically
        GUILayout.BeginVertical();
        if (GUILayout.Button("^", GUILayout.Height(14), GUILayout.Width(25)))
        {
            OnCLickLayerUp();
        }
        if (GUILayout.Button("v", GUILayout.Height(14), GUILayout.Width(25)))
        {
            OnCLickLayerDown();
        }
        GUILayout.EndVertical();
        if (GUILayout.Button(layersList[currentLayer].name, GUILayout.Height(70), GUILayout.Height(30)))
        {
            // Handle "Current Layer" button click
        }
        GUILayout.EndHorizontal();

        // "Isolate Layer" button
        if (GUILayout.Button("Isolate Layer", GUILayout.ExpandWidth(true)))
        {
            OnCLickIsolateLayer();
        }

        //Paint Mode 
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();

        if (GUILayout.Button("^", GUILayout.Height(10), GUILayout.Width(25)))
        {
            OnCLickPaintInFront();
        }
        if (GUILayout.Button("=", GUILayout.Height(10), GUILayout.Width(25)))
        {
            OnCLickPaintMiddle();
        }
        if (GUILayout.Button("v", GUILayout.Height(10), GUILayout.Width(25)))
        {
            OnCLickPaintBehind();
        }
        GUILayout.EndVertical();
        GUILayout.Button(paintMode.ToString(), GUILayout.ExpandWidth(true), GUILayout.Height(30));
        GUILayout.EndHorizontal();

        // "Camera Mode" button
        if (GUILayout.Button("Camera Mode", GUILayout.ExpandWidth(true)))
        {
            OnCLickToggleCameraMode();
        }
        DrawLayerHotkeys();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
    private void DrawBrushHotkeys()
    {
        GUILayout.Label(
            "Hold 'E' to ROTATE the last painted prop\n" +
            "Ctrl+Z to DESTROY the last painted prop\n" +
            "Z Clear Stamp\n" +
            "alt-X use previous brush\n" +
            "L to flip the sprite\n"
            );
    }
    private void DrawLayerHotkeys()
    {
        GUILayout.Label(
            "- go back a layer\n" +
            "+ go forward a layer\n" +
            "[ go back in layer\n" +
            "] go forward in layer\n" +
            "| go to middle of layer\n" +
            "alt+[ paint behind\n" +
            "alt+] paint infront\n" +
            "alt+| paint middle\n" +
            "H isolate layer"
            );
    }
    private void DrawBrushButtons()
    {
        GUILayout.BeginArea(new Rect(10,
             10,
             position.width / 3,
             position.height / 2)
             );

        // Create a group of vertical buttons
        GUILayout.BeginVertical();
        if (GUILayout.Button("Undo", GUILayout.Width(40), GUILayout.Height(25)))
        {
            // Handle "Undo" button click
        }
        //Current Brush Icon
        Texture2D icon = AssetPreview.GetAssetPreview(selectedStamp);
        //initialize our objects
        GUIContent content = new GUIContent();
        GUIStyle iconStyle = new GUIStyle();
        if (icon != null)
        {
            content.image = icon;
            content.tooltip = selectedStamp.name;

        }

        //get rid of button hover changes
        iconStyle.normal.background = null;
        iconStyle.hover.background = null;
        GUILayout.Label("Current Stamp");
        GUILayout.Label(content, iconStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

        //current brush
        //clear current brush
        //last placed prefab
        //jitter settings
        //Current Brush Icon
        Texture2D lastPlacedPrefabIcon = AssetPreview.GetAssetPreview(lastPlacedPref);
        if (lastPlacedPrefabIcon != null)
        {
            content.image = lastPlacedPrefabIcon;
            content.tooltip = lastPlacedPref.name;

        }

        //get rid of button hover changes

        //GUILayout.Label(content, iconStyle, GUILayout.Width(64), GUILayout.Height(64));
        GUILayout.Label("Last Placed Prefab");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(content, iconStyle, GUILayout.Width(64), GUILayout.Height(64)))
        {
            OnClickLastPlacedPrefab();
        }

        if (GUILayout.Button("Clear Current Stamp"))
        {
            ClearStamp();
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
    private void DrawGrids()
    {
        //create prefab grids
        GUILayout.BeginArea(new Rect(0f,
            position.height / 2 + 10,
            position.width,
            position.height / 2)
            );
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        //foreach (Tuple<String,List<GameObject>> t in SubfolderPrefabLists)
        for (int i = 0; i < stampFolders.Count; i++)
        {
            if (GUILayout.Button(stampFolders[i].Name))
            {
                stampFolders[i].Active = !stampFolders[i].Active;
            }

            if (stampFolders[i].Active)
            {
                DrawPrefabIconGrid(stampFolders[i].Stamps, prefabButtonSize, (int)position.width / prefabButtonSize);
            }
        }
        EditorGUILayout.EndScrollView();

        GUILayout.EndArea();
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
                GUIStyle iconStyle = new GUIStyle();
                //get rid of button hover changes
                iconStyle.normal.background = null;
                iconStyle.hover.background = null;
                if (icon != null)
                {
                    content.image = icon;
                    content.tooltip = prefab.name;
                }

                if (GUILayout.Button(content, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
                {
                    lastBrush = selectedStamp;
                    selectedStamp = prefab;
                    Debug.Log("Selected prefab is " + prefab.name);
                }
            }

            if ((i + 1) % itemsPerRow == 0 || i == prefabList.Count - 1)
            {
                GUILayout.EndHorizontal();
            }
        }

        Repaint();
        GUILayout.EndVertical();
    }
    #endregion DrawingGUI

    #region ButtonFunctions

    private void OnCLickUndo()
    {
        if (lastPlacedPref != null && prefabHistory.Peek() != null)
        {
            DestroyImmediate(prefabHistory.Pop());
            lastPlacedPref = null;
            if (prefabHistory.Count > 0)
            {
                lastPlacedPref = prefabHistory.Peek();
            }
        }
    }
    private void OnCLickIsolateLayer()
    {
        if (!hideMode)
        {
            SceneVisibilityManager.instance.HideAll();
            SceneVisibilityManager.instance.Show(layersList[CurrentLayer], true);
            hideMode = true;
        }
        else
        {
            SceneVisibilityManager.instance.ShowAll();
            hideMode = false;
        }
    }
    private void OnCLickToggleCameraMode()
    {
        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView == null) { return; }

        sceneView.in2DMode = !sceneView.in2DMode;
        if (!sceneView.in2DMode)
        {
            sceneView.orthographic = false;
            sceneView.rotation = Quaternion.LookRotation(Vector3.forward);
        }


        SceneView.RepaintAll();
    }
    private void OnCLickPaintBehind()
    {
        paintMode = PaintMode.PaintBehind;
        Repaint();
    }
    private void OnCLickPaintMiddle()
    {
        paintMode = PaintMode.PaintMiddle;
        Repaint();
    }
    private void OnCLickPaintInFront()
    {
        paintMode = PaintMode.PaintInFront;
        Repaint();
    }
    private void OnCLickLayerUp()
    {
        CurrentLayer = CurrentLayer + 1;
        UpdateLayers();
    }
    private void OnCLickLayerDown()
    {
        CurrentLayer = CurrentLayer - 1;
        UpdateLayers();
    }


    private void UpdateLayers()
    {
        //disable clicking on all but the current layer
        SceneVisibilityManager.instance.DisableAllPicking();
        SceneVisibilityManager.instance.EnablePicking(layersList[currentLayer], true);
        foreach (GameObject l in layersList)
        {
            SandolkakosDigital.EditorUtils.SceneHierarchyUtility.SetExpandedRecursive(l, false);
        }
        SandolkakosDigital.EditorUtils.SceneHierarchyUtility.SetExpandedRecursive(layersList[currentLayer], true);
        this.Focus();
        if (hideMode)
        {
            SceneVisibilityManager.instance.HideAll();
            SceneVisibilityManager.instance.Show(layersList[currentLayer], true);

        }
        EditorApplication.RepaintHierarchyWindow();
        Repaint();
    }
    private void MoveBackInLayer()
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
    private void MoveForwardInLayer()
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
    private void MoveMiddleInLayer()
    {
        if (Selection.activeTransform != null)
        {
            Selection.activeTransform.SetSiblingIndex(Selection.activeTransform.parent.childCount / 2);
        }
        else
        {
            lastPlacedPref.transform.SetSiblingIndex(Selection.activeTransform.parent.childCount/2);
        }
    }
    private void ClearStamp()
    {
        selectedStamp = null;
        Repaint();
    }
    private void UseLastBrush()
    {
        GameObject tmp = selectedStamp;
        selectedStamp = lastBrush;
        lastBrush = tmp;
        Repaint();
    }
    private void FlipPrefabX()
    {
        if (lastPlacedPref != null)
        {
            lastPlacedPref.GetComponent<SpriteRenderer>().flipX = !lastPlacedPref.GetComponent<SpriteRenderer>().flipX;
        }
    }
    private void FlipPrefabY()
    {
        if (lastPlacedPref != null)
        {
            lastPlacedPref.GetComponent<SpriteRenderer>().flipY = !lastPlacedPref.GetComponent<SpriteRenderer>().flipY;
        }
    }
    private void OnClickLastPlacedPrefab()
    {
        if (lastPlacedPref == null) { return; }
        Selection.activeObject = lastPlacedPref;
        SceneView.FrameLastActiveSceneView();
    }

    private void Paint()
    {
        lastPlacedPref = PrefabUtility.InstantiatePrefab(selectedStamp) as GameObject;
        lastPlacedPref.transform.position = new Vector3(GetMousePosition().x, GetMousePosition().y, 0);
        lastPlacedPref.transform.parent = layersList[CurrentLayer].transform.GetChild(0);
        switch (paintMode)
        {
            case PaintMode.PaintBehind:
                lastPlacedPref.transform.SetAsFirstSibling();
                break;
            case PaintMode.PaintMiddle:
                lastPlacedPref.transform.SetSiblingIndex(lastPlacedPref.transform.parent.childCount / 2);
                break;
            case PaintMode.PaintInFront:
                lastPlacedPref.transform.SetAsLastSibling();
                break;
        }

        if (prefabHistory.Count > 40)
        {//remove first item
            prefabHistory.Reverse();
            prefabHistory.Pop();
            prefabHistory.Reverse();
        }
        prefabHistory.Push(lastPlacedPref);
        ApplyBrushToObject(lastPlacedPref);
        Selection.activeObject = lastPlacedPref;
        placedObjectLastFrame = true;
    }
    #endregion ButtonFunctions

    #region Utils
    public Vector3 GetMousePosition()
    {
        Event e = Event.current;
        if (e == null) { return Vector3.zero; }
        Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);   
        float z = 0; // Set z to 0 for 2D games
        return new Vector3(worldRay.origin.x, worldRay.origin.y, z);
    }
    
    public void SetBrush(BrushData brushData)
    {
        CurrentBrush = brushData;
    }

    private void ApplyBrushToObject(GameObject o)
    {
        //randomize jitter ammount within range
        if (CurrentBrush == null)
        {
            CurrentBrush = GetWindow<EnvironmentBrushPallet>().CurrentBrush;
        }
        if (CurrentBrush == null) { Debug.Log("No Brush Active"); return; }
        float hueJitterRange = UnityEngine.Random.Range(-CurrentBrush.HueJitterRange, CurrentBrush.HueJitterRange);
        float saturationJitterRange = UnityEngine.Random.Range(0,CurrentBrush.SaturationJitterRange);
        float brightnessJitterRange = UnityEngine.Random.Range(0,CurrentBrush.BrightnessJitterRange);

        float scaleJitterRange = UnityEngine.Random.Range(1f - (CurrentBrush.ScaleJitterRange * 0.15f), 1f + (CurrentBrush.ScaleJitterRange * 0.15f));

        float rotationJitterRange = UnityEngine.Random.Range(-CurrentBrush.RotationJitterRange, CurrentBrush.RotationJitterRange);
        //float positionOffset = UnityEngine.Random.Range(0, CurrentBrush.scatterRange);
        if (false)
        {
            Debug.Log("Scale Jitter Range: " + scaleJitterRange);
            Debug.Log("Rotation Jitter Range: " + rotationJitterRange);
            Debug.Log("Saturation Jitter Range: " + saturationJitterRange);
            Debug.Log("Brightness Jitter Range: " + brightnessJitterRange);
            Debug.Log("Hue Final Value: " + (hueJitterRange + 1) / 2f);
        }

        //randomize attributes by jitter ammount
        SpriteRenderer sprite = o.GetComponent<SpriteRenderer>();
        sprite.color = Color.HSVToRGB((hueJitterRange + 1) / 2f, saturationJitterRange, 1-brightnessJitterRange);
        o.transform.localScale = new Vector3(scaleJitterRange, scaleJitterRange, 1);
        o.transform.RotateAroundLocal(Vector3.back, rotationJitterRange * Mathf.Deg2Rad);
       // o.transform.position += positionOffset;

        //randomize flipping
        if (CurrentBrush.RandomFlipX) {
            sprite.flipX = UnityEngine.Random.value > 0.5f;
        }
        if (CurrentBrush.RandomFlipY)
        {
            sprite.flipY = UnityEngine.Random.value > 0.5f;
        }
    }

    #endregion Utils

}
#endif
