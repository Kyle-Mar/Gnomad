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

    [SerializeField] private Sprite[] flaskLevels;
    [SerializeField] private GameObject flaskContainer;


    private Health playerHealth;
    private Flask playerFlask;
    public List<GameObject> hearts = new();//if this does not behave correctly, try UnityEngine.UIElements.Image
    private GameObject flaskObject;

    private void Awake()
    {
        Assert.IsNotNull(player);
        Assert.IsTrue(flaskLevels.Length == 4);
        // Moved Player Health Component into a new GameObject
        playerHealth = player.GetComponentInChildren<Health>();
        playerFlask = player.GetComponent<Flask>();
        PopulateUIHearts();
        CreateFlask();
    }

    private void OnEnable()
    {
        playerHealth.onDamage += UIRemoveHealth;
        playerHealth.onHeal += UIAddHealth;
        playerFlask.onFlaskUpdate += UpdateUIFlask;
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

    void CreateFlask()
    {
        GameObject imgObject = new GameObject("Flask");
        Image image = imgObject.AddComponent<Image>();
        
        //Get Max level flask
        image.sprite = flaskLevels[3];
        imgObject.transform.SetParent(flaskContainer.transform);
        if (imgObject.TryGetComponent(out RectTransform rect))
        {
            rect.sizeDelta = new(500, 500);
            rect.anchoredPosition = Vector2.zero;
        }
        flaskObject = imgObject;
    }

    private void CreateHeart(bool heartFull)
    {
        GameObject imgObject = new GameObject("Heart");
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

    public void UIRemoveHealth(float amount, Collider2D collider, Vector3 dir)
    {
        UpdateUIHealth();
    }
    public void UIAddHealth(float amount, Collider2D collider, Vector3 dir)
    {
        UpdateUIHealth();
    }

    private void UpdateUIHealth()
    {
        int playerCurrentHealth = (int)playerHealth.health;
        Debug.Log(playerCurrentHealth);
        //int playerMaxHealth = (int)playerHealth.MaxHealth;

        for (int i = 0; i < hearts.Count; i++)
        {
            if (i > playerHealth.MaxHealth) { return; }
            if (i >= playerCurrentHealth) { 
                hearts[i].GetComponent<Image>().sprite = emptyHeartSprite;
            }
            else{ 
                hearts[i].GetComponent<Image>().sprite = fullHeartSprite; 
            }
        }
    }

    void UpdateUIFlask()
    {
        flaskObject.GetComponent<Image>().sprite = flaskLevels[playerFlask.FlaskLevel];
    }

}
