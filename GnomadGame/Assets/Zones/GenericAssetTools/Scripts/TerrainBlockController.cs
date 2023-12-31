#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainBlockController : MonoBehaviour
{
    [Tooltip("Size given in 128x128 tiles")]
    [SerializeField] Vector2 size;
    private Vector2 previousSize;

    [Tooltip("Draw lineart cap on these borders of the block")]
    public bool CapTop, CapBottom, CapLeft, CapRight;

    private const uint minBlockHeight = 5;
    private const uint maxBlockHeight = 10;
    private const uint minBlockWidth = 5;
    private const uint maxBlockWidth = 10;

    [SerializeField] private Sprite VxVBlock;
    [SerializeField] private Sprite XxVBlock;
    [SerializeField] private Sprite VxXBlock;
    [SerializeField] private Sprite XxXBlock;

    private List<GameObject> blocks = new List<GameObject>();
    /*
        ----Plan----
    Terrain blocks should be top left justified

    Build rows 
    two row sizes, 5 and 10 tiles tall
    fill with the largest sizes available, then move to small size
    tiles must be provided with these sizes
    5x5
    5x10
    10x10
    10x5

    */
    private void OnValidate()
    {
        //only run when size is updated
        if (previousSize == size) { return; }
        //only run when we are within our quantom range
        if ( size.x % minBlockHeight != 0 || size.y % minBlockHeight != 0) { return; }
        previousSize = size;
        if (transform.childCount > 0) { DestroyCurrentBlocks(); }

        Debug.Log(blocks);
        //BuildTerrain();
    }

    private void DestroyCurrentBlocks()
    {
        GameObject currentBlock = transform.GetChild(0).gameObject;
        while (currentBlock != null)
        {   //https://forum.unity.com/threads/onvalidate-and-destroying-objects.258782/
            //idk how this works, but it's from here
            return;
            UnityEditor.EditorApplication.delayCall += () =>
            {
                DestroyImmediate(currentBlock);
            };
        }
    }

    private void BuildTerrain()
    {
        for(uint y=0; y < size.y;)
        {
            if (y + maxBlockHeight <= size.y)
            {
                BuildTerrainRow(maxBlockHeight,y);
                y+=maxBlockHeight;
            }
            else
            {
                BuildTerrainRow(minBlockHeight,y);
                y += minBlockHeight;
            }
        }
    }

    //returns the height of the row built
    private void BuildTerrainRow(uint height, uint y)
    {
        Debug.Log("Building Terrain Row with height " + height);
        for(uint x = 0; x < size.x;)
        {
            if (x+maxBlockWidth <= size.x)
            {
                //create a block that is 10xheight
                CreateBlock(
                    new Vector2(maxBlockWidth, maxBlockHeight),
                    new Vector2 (x,y));
                x += maxBlockWidth;
            }
            else
            {
                //create a block that is 5xheight
                x += minBlockWidth;
            }
        }
    }

    //size in tiles, and position in tile space
    private void CreateBlock(Vector2 blockSize, Vector2 blockPosition)
    {
        blockPosition *= 128;
        GameObject blockObject = new GameObject("BlockSprite");
        SpriteRenderer spriteRenderer = blockObject.AddComponent<SpriteRenderer>();
        blockObject.transform.parent = transform;
        blocks.Add(blockObject);
        Debug.Log("Creating block");
        if (blockSize.x == maxBlockHeight)
        {
            if (blockSize.y == maxBlockWidth)
            {//10x10
                spriteRenderer.sprite = XxXBlock;
            }
            else
            {//5x10
                spriteRenderer.sprite = VxXBlock;
            }
        }
        else
        {
            if (blockSize.y == maxBlockWidth)
            {//10x5
                spriteRenderer.sprite = XxVBlock;
            }
            else
            {//5x5
                spriteRenderer.sprite = VxVBlock;
            }
        }
        blockObject.transform.position += (Vector3)spriteRenderer.size * 0.5f;
    }
    

}
#endif