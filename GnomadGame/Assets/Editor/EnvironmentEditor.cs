using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PrefabPopupMenu : EditorWindow
{
    private static string prefabFolderPath = "Assets/Zones/ForestZone/Environmental Assets/Elements/Foliage";
    private List<GameObject> prefabList = new List<GameObject>();
    private Vector2 scrollPosition;
    private GameObject selectedPrefab;

    [MenuItem("Window/Prefab Popup Menu")]
    static void Init()
    {
        PrefabPopupMenu window = GetWindow<PrefabPopupMenu>();
        window.minSize = new Vector2(200, 200);
        window.Show();
    }

    private void OnEnable()
    {
        GetPrefabs();
    }

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
    }

    private void OnGUI()
    {
        GUILayout.Label("Prefab Popup Menu");
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        foreach (GameObject prefab in prefabList)
        {
            DrawPrefabIcon(prefab);
        }

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Clear Brush"))
        {
            ClearBrush();
        }
    }

    private void DrawPrefabIcon(GameObject prefab)
    {
        if (prefab != null)
        {
            GUIContent content = new GUIContent();
            Texture2D icon = AssetPreview.GetAssetPreview(prefab);

            if (icon != null)
            {
                content.image = icon;
                content.tooltip = prefab.name;
            }

            if (GUILayout.Button(content, GUILayout.Width(64), GUILayout.Height(64)))
            {
                selectedPrefab = prefab;
            }
        }
    }

    private void ClearBrush()
    {
        selectedPrefab = null;
    }

    private void OnSceneGUI()
    {
        Event currentEvent = Event.current;

        if (selectedPrefab != null && currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject instantiatedPrefab = PrefabUtility.InstantiatePrefab(selectedPrefab) as GameObject;
                instantiatedPrefab.transform.position = hit.point;
                currentEvent.Use();
            }
        }
    }
}
