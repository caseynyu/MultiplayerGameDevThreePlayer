using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CursorUI : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private RectTransform customCursorVisual; // Your custom UI cursor image
    [SerializeField] private GraphicRaycaster graphicRaycaster; // Drag your Canvas GraphicRaycaster here
    
    private EventSystem eventSystem;
    private PointerEventData pointerData;
    private GameObject currentHoveredObject;

    void Start()
    {
        // Cache the active event system
        eventSystem = EventSystem.current;
        
        // Initialize custom pointer event data
        pointerData = new PointerEventData(eventSystem);
    }

    void Update()
    {
        // 1. Update the virtual pointer screen position to match your visual UI cursor
        Vector2 cursorScreenPosition = RectTransformUtility.WorldToScreenPoint(null, customCursorVisual.position);
        pointerData.position = cursorScreenPosition;

        // 2. Perform a Graphic Raycast against the UI elements
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerData, results);

        // Find the closest eligible UI object hit by the raycast
        GameObject hitObject = results.Count > 0 ? results[0].gameObject : null;

        // 3. Handle Pointer Enter / Exit (Hover States)
        HandleHoverStates(hitObject);

        Gamepad gamepad = Gamepad.current;
        if (gamepad == null) return;

        // 4. Handle Clicks (Replace with your custom input system checking, e.g., Gamepad Button South)
        if (gamepad.buttonSouth.wasPressedThisFrame)
        {
            TriggerPointerDown(hitObject);
        }
        if (gamepad.buttonSouth.wasReleasedThisFrame)
        {
            TriggerPointerUp(hitObject);
        }
    }

    private void HandleHoverStates(GameObject hitObject)
    {
        if (currentHoveredObject != hitObject)
        {
            // Exit previous object
            if (currentHoveredObject != null)
            {
                ExecuteEvents.Execute(currentHoveredObject, pointerData, ExecuteEvents.pointerExitHandler);
            }

            // Enter new object
            currentHoveredObject = hitObject;
            if (currentHoveredObject != null)
            {
                ExecuteEvents.Execute(currentHoveredObject, pointerData, ExecuteEvents.pointerEnterHandler);
            }
        }
    }

    private void TriggerPointerDown(GameObject target)
    {
        if (target == null) return;

        pointerData.pressPosition = pointerData.position;
        pointerData.pointerPressRaycast = pointerData.pointerCurrentRaycast;
        pointerData.pointerPress = ExecuteEvents.ExecuteHierarchy(target, pointerData, ExecuteEvents.pointerDownHandler);
    }

    private void TriggerPointerUp(GameObject target)
    {
        if (target == null || pointerData.pointerPress == null) return;

        // Execute Pointer Up
        ExecuteEvents.Execute(pointerData.pointerPress, pointerData, ExecuteEvents.pointerUpHandler);

        // Check if we released the click over the same object we started clicking on (A formal Click)
        GameObject pointerClickHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(target);
        if (pointerData.pointerPress == pointerClickHandler)
        {
            ExecuteEvents.Execute(pointerData.pointerPress, pointerData, ExecuteEvents.pointerClickHandler);
        }

        // Reset press references
        pointerData.pointerPress = null;
    }
}