using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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
        pointer.position=(GetComponent<RectTransform>().position);

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointer, raycastResults);
        Debug.Log(transform.position);
        foreach (RaycastResult raycastResult in raycastResults)
        {
            Debug.Log(raycastResult);
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
