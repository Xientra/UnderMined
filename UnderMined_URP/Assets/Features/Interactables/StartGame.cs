using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : Interactable
{
    [SerializeField] private DrillController drill;
    public override void Interact(CharacterInputController player)
    {
        if(!player.pickUp && GameManager.instance.gameIsRunning)
        {
            this.gameObject.AddComponent<SteeringWheel>();
            this.GetComponent<SteeringWheel>().drill = drill;
            this.gameObject.SetActive(false);
            this.gameObject.SetActive(true);
            Destroy(this);

            GameManager.instance.StartGame();
        }

    }
}
