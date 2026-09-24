using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public sealed class ControllerPointerEventData : PointerEventData
{
    public int PlayerIndex { get; }
    public ControllerPointerEventData(EventSystem eventSystem, int playerIndex) : base(eventSystem)
    {
        PlayerIndex = playerIndex;
        pointerId = -100 - playerIndex;
        button = InputButton.Left;
    }
}

public class MouseClickCursorGamepad : MonoBehaviour
{
    [SerializeField] int gamepadSelection;
    private GameManager gameManager;

    private void Awake() => gameManager = FindAnyObjectByType<GameManager>();

    private void Start()
    {
        // Keep virtual mouse motion, but send clicks ourselves with a player identity.
        var virtualMouse = GetComponent<VirtualMouseInput>();
        if (virtualMouse != null) virtualMouse.leftButtonAction = default;
    }

    private void LateUpdate()
    {
        if (gameManager == null || !gameManager.IsBuilding) return;
        var gamepad = GameManager.GetGamepad(gamepadSelection);
        if (gamepad == null || !gamepad.buttonSouth.wasPressedThisFrame) return;
        var target = GetClickTarget(gameObject, gamepadSelection, out var pointer);
        if (target != null) ExecuteEvents.Execute(target, pointer, ExecuteEvents.pointerClickHandler);
    }

    private static Vector2 ScreenPosition(GameObject cursor)
    {
        var canvas = cursor.GetComponentInParent<Canvas>();
        var camera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? canvas.worldCamera : null;
        return RectTransformUtility.WorldToScreenPoint(camera, cursor.transform.position);
    }

    public static Vector3 WorldPosition(GameObject cursor)
    {
        var ray = Camera.main.ScreenPointToRay(ScreenPosition(cursor));
        var plane = new Plane(Vector3.forward, Vector3.zero);
        return plane.Raycast(ray, out float distance) ? ray.GetPoint(distance) : Vector3.zero;
    }

    public static GameObject GetClickTarget(GameObject cursor, int playerIndex, out ControllerPointerEventData pointer)
    {
        pointer = null;
        if (EventSystem.current == null) return null;
        pointer = new ControllerPointerEventData(EventSystem.current, playerIndex) { position = ScreenPosition(cursor) };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, results);
        if (results.Count == 0) return null;
        pointer.pointerCurrentRaycast = results[0];
        return ExecuteEvents.GetEventHandler<IPointerClickHandler>(results[0].gameObject);
    }
}
