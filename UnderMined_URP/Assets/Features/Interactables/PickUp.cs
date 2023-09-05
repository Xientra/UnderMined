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
    

    public override bool Interact(CharacterInputController player) //pick up
    {
        if (!player.pickUp)
        {
            player.pickUp = this;
            transform.position = player.transform.forward;
            transform.SetParent(player.transform);
        }
        else if (player.pickUp.Type == Type && player.pickUp.amount < player.maxCarryAmount)
        {
            player.pickUp.amount += amount;
            Destroy(gameObject);
        }
        else
        {
            //do error sound
            return false;
        }

        return true;
    }
}
