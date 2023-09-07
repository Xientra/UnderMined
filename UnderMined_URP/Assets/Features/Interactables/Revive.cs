using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Revive : Interactable
{
    public CharacterInputController fallenplayer;
    public override void Interact(CharacterInputController player)
    {
        player.isFallen = false;
        Destroy(this.gameObject);
    }
}
