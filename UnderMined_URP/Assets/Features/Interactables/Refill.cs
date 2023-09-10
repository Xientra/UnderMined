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
            if(player.pickUp.wallType == WallType.Coal)
            {
                drill.AddCoal(player.pickUp.amount);
                
            }
            else if (player.pickUp.wallType == WallType.Gold)
            {
                GameManager.instance.gold += player.pickUp.amount;
            }
            else if (player.pickUp.wallType == WallType.Iron)
            {
                //idk
            }
            Destroy(player.pickUp.gameObject);
            player.currentInteractable = null;
        }
    }
}
