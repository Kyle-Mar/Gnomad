using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MainMenuController : MonoBehaviour
{
public void onClickPlay()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
        SceneManager.LoadScene(2);
        //SceneManager.UnloadScene(0);
    }


    public void onClickOptions()
    {
        Debug.Log("Options Menu Not Implemented");
    }

public void onClickExit()
    {
        Application.Quit();
    }


}
