using UnityEngine;
using UnityEngine.InputSystem;

public class MouseClickCursorGamepad : MonoBehaviour
{
    [SerializeField] int gamepadSelection;
    private GameManager gameManager;
    void Awake()
    {
        gameManager = GameObject.FindAnyObjectByType<GameManager>();
    }

    void Update()
    {
        if (Gamepad.all[gamepadSelection].buttonSouth.wasPressedThisFrame)
        {
            gameManager.lastPressedGamepadMouse=gamepadSelection;
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
            Mouse.current.WarpCursorPosition(new Vector2(screenPoint.x, screenPoint.y));
            
        }
        
    }
    
}
