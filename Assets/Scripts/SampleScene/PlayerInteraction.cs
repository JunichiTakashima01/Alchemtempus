using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public static event Action OnPlayerInteraction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void Interact(InputAction.CallbackContext context)
    {
        OnPlayerInteraction.Invoke();
    }
}
