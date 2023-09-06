using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringWheel : Interactable
{
    [SerializeField] public DrillController drill;

    private void Awake()
    {
        this.enabled = false;
    }

    public override void Interact(CharacterInputController player)
    {
        if(!player.pickUp && drill.isRunning)
        {
            player.isSteeringDrill = true;
            player._drillController = drill;
            player.transform.position = this.gameObject.transform.position - (this.gameObject.transform.forward);
            player.transform.SetParent(drill.gameObject.transform);
            player.currentInteractable = null;
        }
    }
}
