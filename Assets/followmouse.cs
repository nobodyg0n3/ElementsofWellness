using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followmouse : MonoBehaviour

{
    void Start()
    {
        
    }

    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 5.0f;

        Vector3 mouseScreenToWorld = Camera.main.ScreenToWorldPoint(mousePosition);
        mouseScreenToWorld.y = 0;
        Vector3 position = Vector3.Lerp(transform.position, mouseScreenToWorld, 1.0f - Mathf.Exp(-8.0f * Time.deltaTime));

        transform.position = position;
        
    }
}
