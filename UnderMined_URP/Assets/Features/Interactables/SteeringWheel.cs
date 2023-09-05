using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringWheel : Interactable
{
    [SerializeField] private DrillController drill;
    public override bool Interact(CharacterInputController player)
    {
        if (!player.pickUp)
        {
            player.isSteeringDrill = true;
            player.transform.position = this.gameObject.transform.position - (-1 * this.gameObject.transform.forward);
            player.transform.SetParent(drill.gameObject.transform);
            drill.Steer(player.moveVec.x);
            return true;
        }

        return false;
    }
}
