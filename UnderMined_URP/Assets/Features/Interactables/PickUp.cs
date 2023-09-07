using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PickUp : Interactable
{
    public enum PickUpType {Coal, Iron, Gold}

    public PickUpType Type;
    
    public float amount = 1.0f;

    public Rigidbody rb;
    public Collider col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public override void Interact(CharacterInputController player) //pick up
    {
        if (!player.pickUp) //player isnt carrying anything
        {
            player.pickUp = this;
            transform.position = player.transform.position + player.transform.forward * 2;
            transform.SetParent(player.transform);
            transform.position += new Vector3(0, 0.8f, 0);
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
    }
}
