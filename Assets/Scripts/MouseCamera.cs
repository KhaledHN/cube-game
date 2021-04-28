using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCamera : MonoBehaviour
{   
    float mouseX;
    float mouseY;

    public float sensitivity = 100f;

    public Transform player;

    float rotation = 0f;

    public float minAngle = -10f;
    public float maxAngle = 20f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        rotation -= mouseY;
        rotation = Mathf.Clamp(rotation, minAngle, maxAngle);
        transform.localRotation = Quaternion.Euler(rotation, 0, 0);
        
        player.Rotate(Vector3.up * mouseX);

    }
}
