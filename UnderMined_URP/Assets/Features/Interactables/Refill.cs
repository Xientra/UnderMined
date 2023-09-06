using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Refill : Interactable
{
    [SerializeField] private DrillController drill;

    public override bool Interact(CharacterInputController player)
    {
        if (player.pickUp.Type == PickUp.PickUpType.Coal)
        {
            drill.AddCoal(player.pickUp.amount);
            Destroy(player.pickUp);
            return true;
        }
        else
        {
            return false;
        }
    }
}
