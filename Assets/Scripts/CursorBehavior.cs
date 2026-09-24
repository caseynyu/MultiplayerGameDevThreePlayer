using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using NUnit.Framework.Interfaces;
using System.Drawing;
using UnityEngine.InputSystem.UI;
using UnityEngine.UIElements;

public class CursorBehavior : MonoBehaviour
{
    [SerializeField]private GraphicRaycaster graphicRaycaster;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] float moveSpeed;
    [SerializeField] GameObject cursorSprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        /*Vector2 move = Gamepad.all[0].leftStick.ReadValue();
        //Debug.Log(move);
        transform.Translate(move * Time.deltaTime * moveSpeed);
        
        var pointer = new PointerEventData(EventSystem.current);
        pointer.position=(transform.position);

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointer, raycastResults);
        Debug.Log(pointer.position);
        foreach (RaycastResult raycastResult in raycastResults)
        {
            //Debug.Log(raycastResult);
            IPointerClickHandler ipcHandler = raycastResult.gameObject.GetComponent<IPointerClickHandler>();
            ipcHandler.OnPointerClick(pointer);
            var ui = raycastResult.gameObject.GetComponent<UIBehaviour>();
            if (ui)
            {
                if (Gamepad.all[0].buttonSouth.isPressed)
                {
                    Debug.Log($"Clicked on {raycastResult.gameObject}");
                    ExecuteEvents.Execute(raycastResult.gameObject, pointer, ExecuteEvents.pointerClickHandler);
                }
            }
        }*/
        Gamepad gamepad = GameManager.GetGamepad(0);
        if (gamepad == null) return;

        Vector2 move = gamepad.leftStick.ReadValue();
        cursorSprite.transform.Translate(move * Time.deltaTime * moveSpeed);
        if (gamepad.buttonSouth.wasPressedThisFrame)
        {
            GameObject clickedUIObject = GetUIObjectAtScreenPosition(cursorSprite.transform.position);

            if (clickedUIObject != null)
            {
                
            }
        }
    }

    public GameObject GetUIObjectAtScreenPosition(Vector2 screenPosition)
    {
        if (eventSystem == null) return null;

        // 3. Set up the Pointer Event Data with the screen position
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = screenPosition;
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(screenPosition);
        Mouse.current.WarpCursorPosition(new Vector2(screenPoint.x, screenPoint.y));

        // 4. Create a list to receive all the raycast results
        List<RaycastResult> results = new List<RaycastResult>();

        // 5. Perform the raycast against the graphics on this Canvas
        graphicRaycaster.Raycast(pointerData, results);

        // 6. Return the first object hit (the one closest to the screen/topmost layer)
        if (results.Count > 0)
        {
            IPointerClickHandler ipcHandler = results[0].gameObject.GetComponent<IPointerClickHandler>();
            ipcHandler.OnPointerClick(pointerData);
            //ExecuteEvents.Execute(results[0].gameObject, pointerData, ExecuteEvents.pointerClickHandler);
            //Debug.Log($"Hit UI Object: {clickedUIObject.name}", clickedUIObject);
            return results[0].gameObject;
        }

        return null;
    }




    
}
