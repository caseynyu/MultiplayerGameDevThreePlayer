using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using NUnit.Framework.Interfaces;
using System.Drawing;

public class CursorBehavior : MonoBehaviour
{
    private GraphicRaycaster graphicRaycaster;
    [SerializeField] float moveSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        graphicRaycaster=GetComponent<GraphicRaycaster>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 move = Gamepad.all[0].leftStick.ReadValue();
        //Debug.Log(move);
        transform.Translate(move * Time.deltaTime * moveSpeed);
        
        var pointer = new PointerEventData(EventSystem.current);
        pointer.position=(GetComponent<RectTransform>().anchoredPosition);

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointer, raycastResults);
        //Debug.Log(raycastResults[0].);
        foreach (RaycastResult raycastResult in raycastResults)
        {
            //Debug.Log(raycastResult);
            IPointerClickHandler ipcHandler = raycastResult.gameObject.GetComponent<IPointerClickHandler>();
            ipcHandler.OnPointerClick(pointer);
            var ui = raycastResult.gameObject.GetComponent<UIBehaviour>();
            if (ui)
            {
                if (Gamepad.current.buttonSouth.isPressed)
                {
                    Debug.Log($"Clicked on {raycastResult.gameObject}");
                    ExecuteEvents.Execute(raycastResult.gameObject, pointer, ExecuteEvents.pointerClickHandler);
                }
            }
        }
    }




    
}
