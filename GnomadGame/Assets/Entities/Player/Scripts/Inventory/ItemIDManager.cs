using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Text;
using System;
using System.IO;




// For some reason, this needs to be right next to BaseItem or it won't compile. Don't ask me, I don't know. - Kyle


#if UNITY_EDITOR
public class ItemIDManager : UnityEditor.AssetModificationProcessor
{
    //public static string ItemIDListEditorPrefs = "ItemIDList";
    public static List<int> UsedItemIDs = new List<int>();

    public static int GetNextAvailableID()
    {
        int nextItemId = 0;
        bool wasGap = false;

        UsedItemIDs = GetItemIdListFromTXT();

        if (UsedItemIDs.Count <= 0)
        {
            Debug.Log("EMPTY");
            nextItemId = 0;
            UsedItemIDs.Add(0);
            //This is disgusting but i'm lazy
            goto buildItemIdString;
        }


        for (int i = 0; i < UsedItemIDs.Count-1; i++)
        {
            Debug.Log("IN LOOP");
            Debug.Log((UsedItemIDs[i]) - UsedItemIDs[i + 1]);
            if(UsedItemIDs[0] != 0)
            {
                wasGap = true;
                nextItemId = 0;
                Debug.Log("PREPEND");
                UsedItemIDs.Insert(0, nextItemId);
                goto buildItemIdString;
            }

            if((UsedItemIDs[i]) - UsedItemIDs[i + 1] < -1)
            {
                Debug.Log("INSERT IN GAP");
                wasGap = true;
                nextItemId = UsedItemIDs[i] + 1;
                UsedItemIDs.Insert(i+1, nextItemId);
                goto buildItemIdString;
                
            }
        }
        if (!wasGap)
        {

            Debug.Log("APPEND");
            nextItemId = UsedItemIDs[UsedItemIDs.Count - 1] + 1;
            UsedItemIDs.Add(nextItemId);
            goto buildItemIdString;
        }
        
        buildItemIdString:
        StringBuilder sb = new();

        foreach(var x in UsedItemIDs)
        {
            sb.Append(x.ToString());
            sb.Append(",");
        }
        WriteToTXT(sb.ToString());

        Debug.Log(nextItemId);

        return nextItemId;
    }

    static void OnWillCreateAsset(string assetName)
    {
       
    }

    static List<int> GetItemIdListFromTXT()
    {
        var listString = ReadFromTXT();
        List<int> tmp = new();
        StringBuilder sb = new();
        for (int i = 0; i < listString.Length - 1; i++)
        {
            while (listString[i] != ',')
            {
                sb.Append(listString[i]);
                i++;
            }
            //at the comma.

            var x = int.Parse(sb.ToString());
            tmp.Add(x);
            sb.Clear();

        }
        return tmp;
    }

    public static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions opt)
    {
        //Debug.Log("HELLO WORLD");

        int itemID;
        if (AssetDatabase.GetMainAssetTypeAtPath(path) != typeof(PlayerInventory.BaseItem))
        {
            return AssetDeleteResult.DidNotDelete;
        }

        UsedItemIDs = GetItemIdListFromTXT();

        var obj = AssetDatabase.LoadMainAssetAtPath(path) as PlayerInventory.BaseItem;
        itemID = obj.ItemID;


        var listString = ReadFromTXT();


        StringBuilder sb = new();

        for(int i = 0; i < listString.Length; i++)
        {
            while(listString[i] != ',')
            {
                sb.Append(listString[i]);
                i++;
            }
            
            //at the comma.
            Debug.Log(i - (sb.Length));
            Debug.Log(sb.ToString().Length + 1);
            if (sb.ToString() == itemID.ToString())
            {
                //account for the comma w/ +1
                listString = listString.Remove(i - (sb.Length), sb.ToString().Length +1);
                
                break;
            }
            else
            {
                sb.Clear();
            }
        }

        UsedItemIDs.Clear();

        for (int i = 0; i < listString.Length - 1; i++)
        {
            while (listString[i] != ',')
            {
                sb.Append(listString[i]);
                i++;
            }
            //at the comma.
            
            var x = int.Parse(sb.ToString());
            UsedItemIDs.Add(x);
            sb.Clear();
            
        }

        WriteToTXT(listString);

        Debug.Log(listString);
        //Exactly the opposite of what you'd expect lmfao unity what the hell?
        return AssetDeleteResult.DidNotDelete;
    }


    public static void WriteToTXT(string stringToWrite)
    {
        TextAsset txt = AssetDatabase.LoadAssetAtPath("Assets/Editor/ItemIDs.txt", typeof(TextAsset)) as TextAsset;
        // Will overwrite current contents
        File.WriteAllText(AssetDatabase.GetAssetPath(txt), stringToWrite);
        EditorUtility.SetDirty(txt);
    }

    public static string ReadFromTXT()
    {
        TextAsset txt = AssetDatabase.LoadAssetAtPath("Assets/Editor/ItemIDs.txt", typeof(TextAsset)) as TextAsset;
        return txt.text;
    }
}

#endif