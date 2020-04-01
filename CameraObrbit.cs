﻿using UnityEngine;
using System.Collections;

public class CameraObrbit : MonoBehaviour {
    public float MinDistance = 1.0f;//was 1
    public float MaxDistance = 1.3f;//was 1.3
    float distance= 1000;
    float distanceTarget;
    Vector2 mouse ;
    Vector2 mouseOnDown ;
    Vector2 rotation;
    Vector2 target =new Vector2(Mathf.PI* 3 / 2, Mathf.PI / 6 );
    Vector2 targetOnDown ; 
    // Use this for initialization
    void Start () {
        distanceTarget = transform.position.magnitude;//get the distance(length of vector) from target

	}
    bool down = false;
    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)//if touchCount is > 0
        {
            if(Input.touches[0].phase== TouchPhase.Began)//when finger touches screen
            {
                down = true;
                mouseOnDown.x = Input.touches[0].position.x;
                mouseOnDown.y = -Input.touches[0].position.y;

                targetOnDown.x = target.x;
                targetOnDown.y = target.y;
            }
            else if(Input.touches[0].phase== TouchPhase.Canceled||//end of finger screen touch
                Input.touches[0].phase== TouchPhase.Ended)
            {
                down = false;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))//if mousebutton down
            {
                down = true;
                mouseOnDown.x = Input.mousePosition.x;
                mouseOnDown.y = -Input.mousePosition.y;

                targetOnDown.x = target.x;
                targetOnDown.y = target.y;
            }
            else if (Input.GetMouseButtonUp(0))//if mousebutton is up
            {
                down = false;
            }
        }
        if(down)
        {
            if (Input.touchCount > 0)
            {
                mouse.x = Input.touches[0].position.x;
                mouse.y = -Input.touches[0].position.y;
            }
            else
            {
                mouse.x = Input.mousePosition.x;
                mouse.y = -Input.mousePosition.y;
            }
            float zoomDamp = distance / 1;//controls speed of orbit

            target.x = targetOnDown.x + (mouse.x - mouseOnDown.x) * 0.005f* zoomDamp;
            target.y = targetOnDown.y + (mouse.y - mouseOnDown.y) * 0.005f* zoomDamp;
            
            target.y = Mathf.Clamp(target.y, -Mathf.PI / 2 + 0.01f, Mathf.PI / 2 - 0.01f);//clamp range for y
            
        }

        distanceTarget -= Input.GetAxis("Mouse ScrollWheel");
        distanceTarget = Mathf.Clamp(distanceTarget, MinDistance, MaxDistance);//settin up min and max values for distanceTarget

        rotation.x += (target.x - rotation.x) * 0.1f;
        rotation.y += (target.y - rotation.y) * 0.1f;
        distance += (distanceTarget - distance) * 0.3f;//controlls how smooth zoom in zoom out is
        Vector3 position;
        position.x = distance * Mathf.Sin(rotation.x) * Mathf.Cos(rotation.y);
        position.y = distance * Mathf.Sin(rotation.y);
        position.z = distance * Mathf.Cos(rotation.x) * Mathf.Cos(rotation.y);
        transform.position = position;
        transform.LookAt(Vector3.zero);
    }
}
