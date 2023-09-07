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
            if(player.pickUp.Type == PickUp.PickUpType.Coal)
            {
                drill.AddCoal(player.pickUp.amount);
                
            }
            else if (player.pickUp.Type == PickUp.PickUpType.Gold)
            {
                GameManager.instance.gold += player.pickUp.amount;
            }
            else if (player.pickUp.Type == PickUp.PickUpType.Iron)
            {
                //idk
            }
            Destroy(player.pickUp.gameObject);
            player.currentInteractable = null;
        }
    }
}
