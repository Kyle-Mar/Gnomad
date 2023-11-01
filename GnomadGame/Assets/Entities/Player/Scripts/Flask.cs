using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerStateMachine))]
public class Flask : MonoBehaviour
{
    // Start is called before the first frame update
    PlayerControls controls;
    [SerializeField] uint flaskLevel = 3;
    [SerializeField] PlayerStateMachine psm;
    [SerializeField] Health playerHealth;
    public uint FlaskLevel => flaskLevel;

    public delegate void OnFlaskUpdate();
    public OnFlaskUpdate onFlaskUpdate;

    private void Start()
    {
        controls = psm.Controls;
        controls.Player.UseFlask.performed += UseFlask;
    }

    private void UseFlask(InputAction.CallbackContext obj)
    {
        if(flaskLevel <= 0)
        {
            return;
        }
        if(playerHealth.health < playerHealth.MaxHealth)
        {
            flaskLevel--;
            playerHealth.Heal(5f);
            onFlaskUpdate?.Invoke();
        }
    }

    private void RefillFlask()
    {
        flaskLevel = 3;
        onFlaskUpdate?.Invoke();
    }

}
