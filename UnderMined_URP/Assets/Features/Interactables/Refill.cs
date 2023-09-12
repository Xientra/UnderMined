using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Refill : Interactable
{
    [SerializeField] private DrillController drill;

    public override void Interact(CharacterInputController player)
    {
        if(player.pickUp)
        {
            drill.AddOre(player.pickUp.wallType, player.pickUp.amount);
            
            Destroy(player.pickUp.gameObject);
            player.currentInteractable = null;
        }
    }
}
