using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class PauseMenuLogic : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    public PlayerControls Controls;

    private void Awake()
    {
        Controls = new PlayerControls();
    }
    private void OnEnable()
    {
        Controls.UI.Pause.performed += OnPauseButtonPressed;
        Controls.Enable();
    }

    private void OnDisable()
    {   //this was causing null reference errors
        Controls.Disable();
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
        StaticUIFunctions.LoadMainMenu();
    }
}
