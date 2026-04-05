using MyGame.Input;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool SprintHeld { get; private set; }
    public bool InteractPressed { get; private set; }
    
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
        
        // controls.Gameplay.Sprint.performed += ctx => SprintHeld = true;
        // controls.Gameplay.Sprint.canceled += ctx => SprintHeld = false;
        //
        // controls.Gameplay.Interact.performed += ctx => InteractPressed = true;
        // controls.Gameplay.Interact.canceled += ctx => InteractPressed = false;
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
}
