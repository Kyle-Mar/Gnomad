
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class MainMenuController : MonoBehaviour
{
    private int currentLevel;
    [SerializeField] private GameObject mainMenuButtons;
    [SerializeField] private GameObject levelSelectButtons;
    //private GameObject currentMenu = mainMenuButtons;

    private void Awake()
    {
        currentLevel = 2;
        Debug.Log("Press play to load Scene " + currentLevel);
    }
    public void OnClickPlay()
    {
        Debug.Log("Loading Master Scene and Scene " + currentLevel);
        SceneManager.LoadScene(currentLevel);
        SceneManager.LoadScene(1, LoadSceneMode.Additive);//Master Scene
        //SceneManager.UnloadScene(0);
    }
    public void OnClickLevelSelect()
    {
        mainMenuButtons.SetActive(false);
        levelSelectButtons.SetActive(true);
    }

    public void OnClickOptions()
    {
        Debug.Log("Options Menu Not Implemented");
    }

    public void OnClickExit()
    {
        Debug.Log("Exit does not work from the editor");
        Application.Quit();
    }

    public void OnClickBack()
    {
        mainMenuButtons.SetActive(true);
        levelSelectButtons.SetActive(false);
    }

    public void OnChangecurrentLevel(System.Single newLevel)
    {
        currentLevel = (int)newLevel;        
        Debug.Log("Press play to load Scene " + currentLevel);

    }
}
