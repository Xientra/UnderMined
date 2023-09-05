using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickUp : Interactable
{
    public enum PickUpType {Coal, Iron, Gold}

    public PickUpType Type;
    
    public float amount = 1.0f;

    public Rigidbody rb;
    public BoxCollider col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
    }

    public override bool Interact(CharacterInputController player) //pick up
    {
        if (!player.pickUp) //player isnt carrying anything
        {
            player.pickUp = this;
            transform.position = player.transform.position + player.transform.forward * 2;
            transform.SetParent(player.transform);
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            transform.rotation = Quaternion.Euler(Vector3.zero);
            col.enabled = false;
            player.currentInteractable = null;
        }
        else if (player.pickUp.Type == Type && player.pickUp.amount < player.maxCarryAmount)
        {
            player.pickUp.amount += amount;
            Destroy(gameObject);
            player.currentInteractable = null;
        }
        else
        {
            //do error sound
            return false;
        }

        return true;
    }
}
