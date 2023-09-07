using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringWheel : Interactable
{
    [SerializeField] public DrillController drill;
    [SerializeField] private Vector3 playerOffset = new Vector3(0, .81f, 0);

    public override void Interact(CharacterInputController player)
    {
        if(!player.pickUp && drill.isRunning)
        {
            player.isSteeringDrill = true;
            player._drillController = drill;
            player.transform.position = this.gameObject.transform.position - (this.gameObject.transform.forward);
            player.transform.SetParent(drill.gameObject.transform);
            player.transform.position = player.transform.position - playerOffset;
            
            player.currentInteractable = null;
        }
    }
}
