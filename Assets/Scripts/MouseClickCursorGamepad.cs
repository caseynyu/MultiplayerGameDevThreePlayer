using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class MouseClickCursorGamepad : MonoBehaviour
{
    [SerializeField] int gamepadSelection;
    private GameManager gameManager;

    void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    void Start()
    {
        // This component sends clicks for its own player. The virtual mouse only moves.
        var virtualMouse = GetComponent<VirtualMouseInput>();
        if (virtualMouse != null)
            virtualMouse.leftButtonAction = default;
    }

    void LateUpdate()
    {
        if (gameManager == null || !gameManager.IsBuilding) return;
        Gamepad gamepad = GameManager.GetGamepad(gamepadSelection);
        if (gamepad == null || !gamepad.buttonSouth.wasPressedThisFrame) return;

        var target = GetClickTarget(gameObject, out var pointer);
        if (target == null) return;

        // Carry the player identity with the event instead of sharing Mouse.current.
        pointer.pointerId = -100 - gamepadSelection;
        gameManager.lastPressedGamepadMouse = gamepadSelection;
        ExecuteEvents.Execute(target, pointer, ExecuteEvents.pointerClickHandler);
    }

    public static Vector2 ScreenPosition(GameObject cursor)
    {
        var canvas = cursor.GetComponentInParent<Canvas>();
        var camera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? canvas.worldCamera : null;
        return RectTransformUtility.WorldToScreenPoint(camera, cursor.transform.position);
    }

    public static Vector3 WorldPosition(GameObject cursor)
    {
        Ray ray = Camera.main.ScreenPointToRay(ScreenPosition(cursor));
        var plane = new Plane(Vector3.forward, Vector3.zero);
        return plane.Raycast(ray, out float distance) ? ray.GetPoint(distance) : Vector3.zero;
    }

    public static GameObject GetClickTarget(GameObject cursor, out PointerEventData pointer)
    {
        pointer = null;
        if (EventSystem.current == null) return null;
        pointer = new PointerEventData(EventSystem.current)
        {
            position = ScreenPosition(cursor),
            button = PointerEventData.InputButton.Left
        };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, results);
        if (results.Count == 0) return null;
        pointer.pointerCurrentRaycast = results[0];
        return ExecuteEvents.GetEventHandler<IPointerClickHandler>(results[0].gameObject);
    }
}
