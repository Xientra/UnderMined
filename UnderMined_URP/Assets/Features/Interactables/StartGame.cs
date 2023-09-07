using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : Interactable
{
    [SerializeField] private DrillController drill;
    public override void Interact(CharacterInputController player)
    {
        if(!player.pickUp)
        {
            this.gameObject.AddComponent<SteeringWheel>();
            this.GetComponent<SteeringWheel>().drill = drill;
            Destroy(this);

            GameManager.instance.StartGame();
        }

    }
}
