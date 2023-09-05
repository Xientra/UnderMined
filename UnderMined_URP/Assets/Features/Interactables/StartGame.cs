using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : Interactable
{
    [SerializeField] private DrillController drill;
    public override bool Interact(CharacterInputController player)
    {
        if(!player.pickUp)
        {
            drill.StartMoving();
            this.gameObject.GetComponent<SteeringWheel>().enabled = true;
            this.gameObject.GetComponent<StartGame>().enabled = false;
            return true;
        }
        else
        {
            return false;
        }
    }
}
