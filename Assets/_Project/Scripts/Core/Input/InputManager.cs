using MyGame.Input;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public GameControls Controls { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        Controls = new GameControls();
        Controls.Enable();
    }

    private void OnDestroy()
    {
        if (Controls != null)
        {
            Controls.Disable();
            Controls.Dispose();
        }
    }

    public Vector2 GetMoveInput() => Controls.Gameplay.Move.ReadValue<Vector2>();
    public Vector2 GetLookInput() => Controls.Gameplay.Look.ReadValue<Vector2>();
    public bool GetJumpPressed() => Controls.Gameplay.Jump.WasPressedThisFrame();

    public void DisableGameplay()
    {
        Controls.Gameplay.Disable();
        Controls.UI.Enable();
    } 
   
    public void EnableGameplay()
    {
        Controls.UI.Disable();
        Controls.Gameplay.Enable();
    }
}
