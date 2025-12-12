using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static InputSystem_Actions;

[CreateAssetMenu(fileName = "New Input Reader", menuName = "Inputs/Input Reader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    public event Action<bool> AttackEvent;
    public event Action<Vector2> MoveEvent;

    public Vector2 LookPosition { get; private set; }
    
    private InputSystem_Actions actions; 
    private void OnEnable()
    {
        if (actions == null)
        {
            actions = new InputSystem_Actions();
            actions.Player.SetCallbacks(this);
        }
        actions.Player.Enable();
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            AttackEvent?.Invoke(true);
        else if (context.canceled)
            AttackEvent?.Invoke(false);
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        LookPosition = context.ReadValue<Vector2>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnNext(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPrevious(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    private void OnDisable()
    {
        actions.Player.Disable();
    }
}

