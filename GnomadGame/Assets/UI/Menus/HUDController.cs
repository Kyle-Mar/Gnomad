using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public List<GameObject> hearts = new();//if this does not behave correctly, try UnityEngine.UIElements.Image

    private void Awake()
    {
        Assert.IsNotNull(player);
        // Moved Player Health Component into a new GameObject
        playerHealth = player.GetComponentInChildren<Health>();
        PopulateUIHearts();
    }

    private void OnEnable()
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
        if(imgObject.TryGetComponent(out RectTransform rect))
        {
            rect.sizeDelta = new Vector2(250, 250); 
        }
        newImage.sprite = fullHeartSprite;
        newImage.transform.SetParent(heartContainer.transform);

        if (heartFull) 
        { newImage.sprite = fullHeartSprite; }
        else 
        { newImage.sprite = emptyHeartSprite; }

        hearts.Add(imgObject);
    }


    public void UIRemoveHealth()
    {
        Debug.Log("OUCH");

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

        for (int i = 0; i < hearts.Count; i++)
        {
            if (i > playerHealth.MaxHealth) { return; }
            if (i >= playerCurrentHealth) { 
                hearts[i].GetComponent<Image>().sprite = emptyHeartSprite;
                hearts[i].name = "EMPTY";
            }
            else{ 
                hearts[i].GetComponent<Image>().sprite = fullHeartSprite; 
            }
        }
    }

}
