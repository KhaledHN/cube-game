using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickupcontroller : MonoBehaviour
{
    public ProjectileGun gunScript;
    public Rigidbody rb;
    public BoxCollider coll;
    public Transform player, gunContainer, fpsCam;

    public float pickUpRange;
    public float dropForwardForce, dropUpwardForce;

    public bool equipped;
    public static bool slotFull;

    private void Update()
    {
        //Check if player is in range and "E" is pressed
        Vector3 distansToPlayer = player.position - transform.position;
        if (!equipped && distansToPlayer.magnitude <= pickUpRange && Input.GetKeyDown(KeyCode.E) && !slotFull) PickUp();

        //Drop if equipped and "Q" is pressed
        if (equipped && Input.GetKeyDown(KeyCode.Q)) Drop();
    }

    private void PickUp()
    {
        equipped = true;
        slotFull = true;

        //Make Rigidbody kinematic and BoxCollider a trigger
        rb.iskinematic = true;
        coll.isTrigger = true;

        //Enable script
        gunScript.enabled = true;
    }

    private void Drop()
    {
        equipped = false;
        slotFull = false;

        //Make Rigidbody not kinematic and BoxCollider a trigger
        rb.iskinematic = false;
        coll.isTrigger = false;

        //disable script
        gunScript.enabled = false;
    }
}
