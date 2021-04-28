using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovment : MonoBehaviour
{
    CharacterController controller;

    public float speed = 8f;
    public float gravity = 20f;
    public float jump = 10f;

    Vector3 movedirection = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.isGrounded)
        {
            movedirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            movedirection *= speed;
            movedirection = transform.TransformDirection(movedirection);

            if (Input.GetButton("Jump"))
            {
                movedirection.y = jump;
            }

        }

        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                speed = 10f;
            }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                speed = 6f;
            }
        }

        movedirection.y -= gravity * Time.deltaTime;
        controller.Move(movedirection * Time.deltaTime);
    }
}
