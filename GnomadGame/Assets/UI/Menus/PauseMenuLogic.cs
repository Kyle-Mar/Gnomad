using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;



public class PauseMenuLogic : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] Button firstSelected;
    public PlayerControls Controls;
     
    private void Awake()
    {
        Controls = new PlayerControls();
    }
    private void OnEnable()
    {
        Controls.UI.Pause.performed += OnPauseButtonPressed;
        Controls.UI.Submit.performed += leave;

        Controls.Enable();
        Button btn = firstSelected;
        EventSystem es = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        es.SetSelectedGameObject(btn.gameObject);
    }

    private void OnDisable()
    {   
        //this was causing null reference errors
    }
    public void OnPauseButtonPressed(InputAction.CallbackContext cxt)
    {
        Debug.Log("Pause Button Pressed");
        StaticUIFunctions.TogglePause(pauseMenu);
    }

    public void OnReloadCurrentScenePressed()
    {
        StaticUIFunctions.ReloadCurrentScene();
    }

    public void OnExitToMainMenuPressed()
    {
        Time.timeScale = 1.0f;
        LevelManager.UnloadAllScenes();
        StaticUIFunctions.LoadMainMenu();
    }

    public void leave(InputAction.CallbackContext cxt)
    {
        if(Time.timeScale != 0 || !pauseMenu.active) { return; }
        OnExitToMainMenuPressed();
    }



}
