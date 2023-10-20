using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


//the main purpose of this file is to share common functions between menus
//and ensure that any menu fuctnionality changes go smoothly without
//leaving behind artifacts
static public class StaticUIFunctions
{
    static public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    static public void TogglePause(GameObject pauseMenu)
    {
        Time.timeScale = 1 - Time.timeScale;
        pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
    }

    static public void ReloadCurrentScene() 
    {
        //StartCoroutine(LevelManager.LoadSceneAsync("reloadScene"));
        throw new NotImplementedException();
    }

    static public void LoadFromLastSave() 
    {
        throw new NotImplementedException();
    }
}

