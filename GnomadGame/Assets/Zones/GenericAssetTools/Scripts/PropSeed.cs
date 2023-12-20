using Mono.Cecil;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
//editor script to randomize small asset locations while furnishing levels
public class SpriteSeeder : MonoBehaviour
{
    [SerializeField] BrushData brushData;
    [SerializeField] GameObject[] propPool;
    GameObject prop;
    [SerializeField] public uint index;
    [SerializeField] bool randomize;
    private void OnEnable()
    {
    }
    private void OnValidate()
    {//if an attribut has been changed, either randomize or set to new index
     //will also randomize when new sprites are added. No efficient way to stop this
     
        if (propPool.Length == 0) { return; }
        if (randomize == true)
        {
            randomize = false;
            uint seed = (uint)UnityEngine.Random.Range(0, uint.MaxValue);
            prop = CreateProp(propPool[seed % propPool.Length]);
        }
        else
        {
            if (prop == null)
            {
                Debug.LogWarning("Sprite Renderer for Sprite Seeder is Null");
                return;
            }
            prop = CreateProp(propPool[index % propPool.Length]);

        }

    }

    GameObject CreateProp(GameObject propRef)
    {
        GameObject newProp = Instantiate<GameObject>(propRef);
        if (brushData != null)
        {
            BrushData.ApplyBrushToObject(brushData, newProp);
        }
        else
        {
            BrushData.ApplyBrushToObject(newProp.GetComponent<Prop>().defaultBrush, newProp);
        }
        return newProp;
    }


}
#endif