/*
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Create some asset folders.
AssetDatabase.CreateFolder("Assets/Meshes", "MyMeshes");
AssetDatabase.CreateFolder("Assets/Prefabs", "MyPrefabs");

// The paths to the mesh/prefab assets.
string meshPath = "Assets/Meshes/MyMeshes/MyMesh01.mesh";
string prefabPath = "Assets/Prefabs/MyPrefabs/MyPrefab01.prefab";

// Delete the assets if they already exist.
AssetDatabase.DeleteAsset(meshPath);
AssetDatabase.DeleteAsset(prefabPath);

// Create the mesh somehow.
Mesh mesh = GetMyMesh();

// Save the mesh as an asset.
AssetDatabase.CreateAsset(mesh, meshPath);
AssetDatabase.SaveAssets();
AssetDatabase.Refresh();

// Create a transform somehow, using the mesh that was previously saved.
Transform trans = GetMyTransform(mesh);

// Save the transform's GameObject as a prefab asset.
PrefabUtility.CreatePrefab(prefabPath, trans.gameObject);
*/