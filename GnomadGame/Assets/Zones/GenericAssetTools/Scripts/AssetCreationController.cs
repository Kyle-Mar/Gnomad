#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class AssetCreationController : MonoBehaviour
{
    [Header("Instanciates the given prefab\nand saves for easy asset creation")]

    [SerializeField]
    [Tooltip("prefab type to be extended from. Must have sprite 2D")]
    GameObject objectAsset;

    [SerializeField]
    [Tooltip("Click to create assets according to the given instructions")]
    Sprite[] sprites;

    [SerializeField]
    [Tooltip("Click to create assets according to the given instructions")]
    bool create;
    [SerializeField]
    bool areYouSure;

    private bool doCreateAssets;

    private void OnValidate()
    {
        if (create == true && areYouSure == true)
        {
            create = areYouSure = false;
            doCreateAssets = true;
        }
    }

    private void Update()
    {
        if (doCreateAssets == true)
        {
            createAssets();
            doCreateAssets = false;
        }
    }

    private void createAssets()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            GameObject tmp = Instantiate(objectAsset, Vector3.zero, Quaternion.identity);
            SpriteRenderer spriteRenderer;
            if (tmp.TryGetComponent(out spriteRenderer))
            {
                spriteRenderer.sprite = sprites[i];
                PrefabUtility.SaveAsPrefabAsset(tmp, "Assets/Zones/GenericAssetTools/PropPrefabs/AutomaticallyGeneratedPrefabs/" + sprites[i].name + ".prefab");
                DestroyImmediate(tmp);
            }
            else
            {
                DestroyImmediate(tmp);
                Debug.Log("Tried to create sprite from prefab with no sprite renderer!");
            }
        }
    }
}
#endif