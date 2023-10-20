
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.Assertions;

public class DeathScreenController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject deathScreenPopup;

    private void OnEnable()
    {
        Assert.IsNotNull(deathScreenPopup);
        // Moved Player Health Component into a new GameObject
        player.GetComponentInChildren<Health>().onDeath += ActivateDeathScreen;
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
