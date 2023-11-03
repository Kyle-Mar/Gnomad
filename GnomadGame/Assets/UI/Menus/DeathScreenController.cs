
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;



public class DeathScreenController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject deathScreenPopup;
    [SerializeField] private Button firstSelected;

    private void OnEnable()
    {
        Assert.IsNotNull(deathScreenPopup);
        // Moved Player Health Component into a new GameObject
        player.GetComponentInChildren<Health>().onDeath += ActivateDeathScreen;
        //Controls.Player.Respawn.performed += ReloadScene;
        Button btn = firstSelected;
        EventSystem es = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        es.SetSelectedGameObject(btn.gameObject);

    }

    private void ActivateDeathScreen()
    {
        deathScreenPopup.SetActive(true);
    }

    private void ExitDeathScreen()
    {
        deathScreenPopup.SetActive(true);
    }
    public void OnClickReturnToMainMenu()
    {
        LevelManager.UnloadAllScenes();
        StaticUIFunctions.LoadMainMenu();
        ExitDeathScreen();
    }

    public void OnClickReloadCurrentScene()
    {
        StaticUIFunctions.ReloadCurrentScene();
        ExitDeathScreen() ;
    }

    public void OnClickReloadFromLastSave()
    {
        StaticUIFunctions.LoadFromLastSave();
        ExitDeathScreen() ;
    }
}
