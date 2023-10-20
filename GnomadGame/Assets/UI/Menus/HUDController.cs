using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Recorder;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class HUDController : MonoBehaviour
{
    [SerializeField] private GameObject player;

    [SerializeField] private Sprite fullHeartSprite, emptyHeartSprite;
    [SerializeField] private GameObject heartContainer;


    private Health playerHealth;
    public Image[] hearts;//if this does not behave correctly, try UnityEngine.UIElements.Image

    private void Awake()
    {
        Assert.IsNotNull(player);
        // Moved Player Health Component into a new GameObject
        playerHealth = player.GetComponentInChildren<Health>();
        PopulateUIHearts();
    }

    private void onEnable()
    {
        playerHealth.onDamage += UIRemoveHealth;
        playerHealth.onHeal += UIAddHealth;
    }

    private void PopulateUIHearts()
    {
        int playerCurrentHealth = (int)playerHealth.health;
        for (int i = 0; i < playerHealth.MaxHealth; i++)
        {
            if (i <= playerCurrentHealth) { CreateHeart(true); }
            else { CreateHeart(false); }
        }
    }

    private void CreateHeart(bool heartFull)
    {
        GameObject imgObject = new GameObject();
        Image newImage = imgObject.AddComponent<Image>();
        newImage.overrideSprite = fullHeartSprite;
        newImage.transform.SetParent(heartContainer.transform);

        if (heartFull) 
        { newImage.sprite = fullHeartSprite; }
        else 
        { newImage.sprite = emptyHeartSprite; }

        hearts.Append(newImage);
    }


    public void UIRemoveHealth()
    {
        UpdateUIHealth();
    }
    public void UIAddHealth()
    {
        UpdateUIHealth();
    }

    private void UpdateUIHealth()
    {
        int playerCurrentHealth = (int)playerHealth.health;
        //int playerMaxHealth = (int)playerHealth.MaxHealth;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i > playerHealth.MaxHealth) { return; }
            if (i <= playerCurrentHealth) { hearts[i].sprite = emptyHeartSprite; }
            else{ hearts[i].sprite = emptyHeartSprite; }
        }
    }

}
