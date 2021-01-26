using StylizedWater;
using System.Collections;
using System.Collections.Generic;
//using System.Windows.Input; //Test, maybe delete
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class SelectHexes : MonoBehaviour
{
    
    [SerializeField] private GameMaster gm;



    Mouse mouse => Mouse.current;

    [SerializeField] private Camera cam;


    private void Start()
    {
        cam.GetComponent<PlanarReflections>().enabled = true;
    }

    public void LeftClick(CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        

        if (Physics.Raycast(cam.ScreenPointToRay(mouse.position.ReadValue()), out RaycastHit hit))
        {
            if (hit.collider == null || EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Hex hex = hit.collider.GetComponent<Hex>();
            if (hex != null && hex.isNotOutOfBounds)
            {
                gm.SetSelectedHex(hex);
            }
        }
    }
}
