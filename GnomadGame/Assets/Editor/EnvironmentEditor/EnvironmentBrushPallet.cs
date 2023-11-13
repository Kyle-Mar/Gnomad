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
using Unity.Mathematics;
using static EnvironmentEditor;
using Entities.Player.Inventory;
using Codice.Client.Common.GameUI;
using System.Drawing;
using NUnit.Framework;
#if UNITY_EDITOR
//TODO
/*

*/
public class EnvironmentBrushPallet : EditorWindow
{
    #region Variables
    private static string brushDirectory = "Assets/Editor/EnvironmentEditor/Brushes/";
    //brush settings
    float hueJitter = 0.3f;
    float saturationJitter = 0.3f;
    float brightnessJitter = 0.3f;
    float rotationJitter = 20f;
    float scaleJitter = 0.1f;

    //float spacingJitter = 0.0f;
    float scatterRange = 0.0f;

    bool randomizeFlipX = true;
    bool randomizeFlipY = false;

    //brush
    String brushName = "";
    String newBrushName = "";
    List<BrushData> brushList = new List<BrushData>();
    BrushData currentBrush = null;
    BrushData currentBrushOriginal = null;

    //GUI Info
    bool assetMenuOpen = false;
    bool variableMenuOpen = false;
    Vector2 scrollPosition = new Vector2();

    #endregion variables

    [MenuItem("Window/Environment Editor/Pallet")]

    #region Initialization
    static void Init()
    {   //Runs once when the Editor is first Opened. Initializes the window
        //This needs to run at a different stage in settup from OnEnable, so it needs to be kept seperate
        EnvironmentBrushPallet window = GetWindow<EnvironmentBrushPallet>();
        window.minSize = new Vector2(300, 300);
        window.Show();
    }

    private void OnEnable()
    {
        PopulateBrushList();
        if (currentBrush == null && brushList.Count > 0)
        {
            InitializeBrushFromFile(brushList[0]);
        }
    }

    #endregion Initialization

    #region Update
    private void OnGUI()
    {
        DrawGUI();
        SetBrushToValues(currentBrush);

    }

    #endregion Update

    #region DrawingGUI

    private void DrawGUI()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        if (GUILayout.Button("Current Brush: " + brushName, GUILayout.Height(40)))
        {
            variableMenuOpen = !variableMenuOpen;
        }
        DrawSliders();
        DrawToggles();

        GUILayout.BeginHorizontal();
        DrawSaveBrushButtons();
        GUILayout.EndHorizontal();
        if (assetMenuOpen)
        {
            DrawAssetMenu();
        }

        DrawBrushList();

        GUILayout.EndScrollView();
    }
    private void DrawSliders()
    {
        if (!variableMenuOpen) { return; }
        GUIContent content = new GUIContent(); // fill in as needed
        int sliderHeight = 20;
        int maxLabelWidth = 125;
        int sliderAmountMaxWidth = 40;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Hue Jitter", GUILayout.Width(maxLabelWidth));
        GUILayout.Label((Mathf.Round(hueJitter * 100f) * 0.01f).ToString(), GUILayout.Width(sliderAmountMaxWidth));
        hueJitter = GUILayout.HorizontalSlider(hueJitter, 0.0f, 1.0f, GUILayout.Height(sliderHeight), GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Saturation Jitter", GUILayout.Width(maxLabelWidth));
        GUILayout.Label((Mathf.Round(saturationJitter * 100f) * 0.01f).ToString(), GUILayout.Width(sliderAmountMaxWidth));
        saturationJitter = GUILayout.HorizontalSlider(saturationJitter, 0.0f, 1.0f, GUILayout.Height(sliderHeight), GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Brightness Jitter", GUILayout.Width(maxLabelWidth));
        GUILayout.Label((Mathf.Round(brightnessJitter * 100f) * 0.01f).ToString(), GUILayout.Width(sliderAmountMaxWidth));
        brightnessJitter = GUILayout.HorizontalSlider(brightnessJitter, 0.0f, 1.0f, GUILayout.Height(sliderHeight), GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Scale Jitter", GUILayout.Width(maxLabelWidth));
        GUILayout.Label((Mathf.Round(scaleJitter * 100f) * 0.01f).ToString(), GUILayout.Width(sliderAmountMaxWidth));
        scaleJitter = GUILayout.HorizontalSlider(scaleJitter, 0.0f, 1.0f, GUILayout.Height(sliderHeight), GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Rotation Jitter", GUILayout.Width(maxLabelWidth));
        GUILayout.Label((Mathf.Round(rotationJitter * 10f) * 0.1f).ToString(), GUILayout.Width(sliderAmountMaxWidth));
        rotationJitter = GUILayout.HorizontalSlider(rotationJitter, 0.0f, 360.0f, GUILayout.Height(sliderHeight), GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();
        /*
        GUILayout.BeginHorizontal();
        GUILayout.Label("Spacing Jitter", GUILayout.Width(maxLabelWidth));
        GUILayout.Label((Mathf.Round(spacingJitter * 10f) * 0.1f).ToString(), GUILayout.Width(sliderAmountMaxWidth));
        spacingJitter = GUILayout.HorizontalSlider(spacingJitter, 0.0f, 1.0f, GUILayout.Height(sliderHeight), GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();
        */
        GUILayout.BeginHorizontal();
        GUILayout.Label("Offset Jitter", GUILayout.Width(maxLabelWidth));
        //GUILayout.Label((Mathf.Round(offsetJitter * 10f) * 0.1f).ToString(), GUILayout.Width(sliderAmountMaxWidth));
        //ffsetJitter = GUILayout.HorizontalSlider(offsetJitter, 0.0f, 1.0f, GUILayout.Height(sliderHeight), GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();

    }
    private void DrawToggles()
    {
        if (!variableMenuOpen) { return; }
        GUIContent content = new GUIContent(); // fill in as needed

        // Toggles go side by side in collumns of two
        int toggleHeight = 30;
        GUILayout.BeginHorizontal();
        content.text = "Randomize Flip X";
        randomizeFlipX = GUILayout.Toggle(randomizeFlipX, content, GUILayout.Height(toggleHeight), GUILayout.ExpandWidth(true));
        content.text = "Randomize Flip Y";
        randomizeFlipY = GUILayout.Toggle(randomizeFlipY, content, GUILayout.Height(toggleHeight), GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();
    }
    private void DrawAssetMenu()    
    {
        if (!variableMenuOpen) { return; }
        GUILayout.Label("Enter Brush Name...");
        newBrushName = GUILayout.TextField(newBrushName);
        if (System.IO.File.Exists(brushDirectory + newBrushName + ".asset"))
        {
            GUILayout.Label("Name In Use. Please Enter a Different Name");
            return;
        }
        if((brushDirectory + newBrushName + ".asset").IndexOfAny(Path.GetInvalidFileNameChars()) < 0)
        {
            GUILayout.Label("Not a valid file name. Contains illegal characters");
            return;
        }
        if (newBrushName == "")
        {
            return; 
        }
        if (!GUILayout.Button("Save Asset", GUILayout.Width(100), GUILayout.Height(30)))
        {
            return;
        }
        //add finalize creation button
        Debug.Log("Creating a new brush");
        BrushData newBrush = ScriptableObject.CreateInstance<BrushData>();
        SetBrushToValues(newBrush);
        SaveNewBrush(newBrush);
        PopulateBrushList();//refresh the brush lisn
        SetBrushName(newBrush);

        newBrushName = "";
    }
    private void DrawBrushList()
    {
        //GUILayout.BeginArea(new Rect(position.x, position.height*2/3, position.width, position.height));
        GUILayout.Label("Select Your Brush Below");

        foreach (BrushData brush in brushList)
        {
            if (GUILayout.Button(brush.ToString().Substring(0, brush.ToString().Length-12)))
            {
                InitializeBrushFromFile(brush);
                
            }
        }
        //GUILayout.EndArea();
    }
    private void DrawSaveBrushButtons()
    {
        if (!variableMenuOpen) { return; }
        if (GUILayout.Button("Save Settings as New brush"))
        {
            assetMenuOpen = !assetMenuOpen;
        }
        if (GUILayout.Button("Apply Settings to Current Brush"))
        {
            SetBrushToValues(currentBrushOriginal);
        }
    }
    #endregion DrawingGUI

    #region ButtonFunctions

    #endregion ButtonFunctions

    #region Utils

    private void PopulateBrushList()
    {
        brushList.Clear();
        string[] guids = AssetDatabase.FindAssets("t:BrushData", new string[] { brushDirectory });

        foreach (String s in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(s);
            BrushData newBD = AssetDatabase.LoadAssetAtPath<BrushData>(assetPath);
            if (newBD != null)
            {
                brushList.Add(newBD);
            }
        }

    }
    private void InitializeBrushFromFile(BrushData b)
    {
        currentBrushOriginal = b;
        SetCurrentBrush(b.Copy());
        SetBrushName(b);
    }
    private void SetCurrentBrush(BrushData b)
    {
        SetValuesFromBrush(b);
        currentBrush = b;
        Assert.IsNotNull(GetWindow<EnvironmentEditor>());
        GetWindow<EnvironmentEditor>().SetBrush(b);
        Focus();

    }
    private void SetValuesFromBrush(BrushData b)
    {
        hueJitter = b.HueJitterRange;
        saturationJitter = b.SaturationJitterRange;
        brightnessJitter = b.BrightnessJitterRange;
        rotationJitter = b.RotationJitterRange;
        scaleJitter = b.ScaleJitterRange;
        String n = b.ToString().Substring(0, b.ToString().Length - 12);
        randomizeFlipX = b.RandomFlipX;
        randomizeFlipY = b.RandomFlipY;
        GetWindow<EnvironmentEditor>().SetBrush(b);
        Focus();
    }
    private void SetBrushToValues(BrushData b)
    {
        b.Init(
            randomizeFlipX, randomizeFlipY,
            hueJitter,
            brightnessJitter,
            saturationJitter,
            scaleJitter,
            rotationJitter
            );
        //SetCurrentBrush(b);
    }
    private void SaveNewBrush(BrushData b)
    {
        AssetDatabase.CreateAsset(b, brushDirectory + newBrushName + ".asset");
        InitializeBrushFromFile(b);
    }

    private void SetBrushName(BrushData b)
    {
        brushName = b.ToString().Substring(0, b.ToString().Length - 12);
    }

    #endregion Utils

}
#endif