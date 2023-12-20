using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUtil : MonoBehaviour
{
    // Start is called before the first frame update


    [SerializeField] bool isPaused = false;
    [SerializeField] bool isSlowed = false;
    float gameSpeed = 1.0f;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            if (isPaused)
            {
                gameSpeed = 1.0f;
                isPaused = false;
            }
            else
            {
                gameSpeed = 0.0f;
                isPaused = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Equals))
        {
            gameSpeed += 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            gameSpeed -= 0.1f;
        }

        gameSpeed = Mathf.Clamp01(gameSpeed);
        Debug.Log(gameSpeed);

        if(gameSpeed > 0f)
        {
            isPaused = false;
        }
        Time.timeScale = gameSpeed;
    }
}
