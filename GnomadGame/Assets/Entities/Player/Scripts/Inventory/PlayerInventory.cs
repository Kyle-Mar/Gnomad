using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
public class PlayerInventory : MonoBehaviour
{
    [SerializeField] int width = 3;
    [SerializeField] int height = 3;
    int[,] matrix;

    private void Awake()
    {
        matrix = new int[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Debug.Log(i +" "+ j);
                if (i % 2 == 0)
                {
                    matrix[i, j] = 0;
                }
                else
                {
                    matrix[i, j] = 1;
                }
            }
        }
        string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        path += "\\debug.txt";

        Debug.Log(path);
        StreamWriter writer = new(path, false);
        writer.Write("HELLO WORLD");

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                writer.Write(matrix[i, j]);
            }
            writer.Write('\n');
        }
        writer.Close();
    }
}
