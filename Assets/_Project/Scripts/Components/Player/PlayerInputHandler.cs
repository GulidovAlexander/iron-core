using System;
using MyGame.Input;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool SprintHeld { get; private set; }
    public bool InteractPressed { get; private set; }
    public bool FireHeld { get; private set; }
    public bool ReloadPressed { get; private set; }
   

    public event Action<int> OnWeaponSwitch;
    
    public event Action OnMenuPressed;
    public event Action OnFireStarted;
    public event Action OnFireStopped;
    public event Action OnReloadPressed;
    
    private GameControls controls;
    
    private void Awake()
    {
        controls = new GameControls();
    }
    
    private void OnEnable()
    {
        controls.Enable();
        
        controls.Gameplay.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => MoveInput = Vector2.zero;
        
        controls.Gameplay.Look.performed += ctx => LookInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Look.canceled += ctx => LookInput = Vector2.zero;
        
        controls.Gameplay.Jump.performed += ctx => JumpPressed = true;
        controls.Gameplay.Jump.canceled += ctx => JumpPressed = false;
        
        controls.Gameplay.Menu.performed += ctx => OnMenuPressed?.Invoke();
        
        controls.Gameplay.Sprint.performed += ctx => SprintHeld = true;
        controls.Gameplay.Sprint.canceled += ctx => SprintHeld = false;
        
        controls.Gameplay.Interact.performed += ctx => InteractPressed = true;
        controls.Gameplay.Interact.canceled += ctx => InteractPressed = false;

        controls.Gameplay.Fire.performed += ctx =>
        {
            FireHeld = true;
            OnFireStarted?.Invoke();
        };
        controls.Gameplay.Fire.canceled += ctx =>
        {
            FireHeld = false;
            OnFireStopped?.Invoke();
        };
        
        controls.Gameplay.Reload.performed += ctx => 
        {
            ReloadPressed = true;
            OnReloadPressed?.Invoke();
        };
        controls.Gameplay.Reload.canceled += ctx => ReloadPressed = false;
        
        controls.Gameplay.Weapon1.performed += ctx => OnWeaponSwitch?.Invoke(0);
        controls.Gameplay.Weapon2.performed += ctx => OnWeaponSwitch?.Invoke(1);
        controls.Gameplay.Weapon3.performed += ctx => OnWeaponSwitch?.Invoke(2);
    }
    
    private void OnDisable()
    {
        controls.Disable();
    }
    
    public bool GetJumpPressed()
    {
        if (JumpPressed)
        {
            JumpPressed = false;
            return true;
        }
        return false;
    }
    
    public bool GetInteractPressed()
    {
        if (InteractPressed)
        {
            InteractPressed = false;
            return true;
        }
        return false;
    }

    public bool GetReloadPressed()
    {
        if (!ReloadPressed) return false;
        ReloadPressed = false;
        return true;
    }
}

