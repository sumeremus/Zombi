using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWorldPosition : MonoBehaviour
{
    Camera mainCamera;
    [SerializeField] Transform weaponObj;
    [SerializeField] LayerMask layerMask;
    void Start()
    {
        mainCamera = Camera.main;
    }


    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            transform.position = hitInfo.point;
        }

        Debug.DrawRay(ray.origin, ray.direction*800, Color.red,0.001f,true);
    }

}
