using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PickUps : Interactable
{
    public enum PickUpType {Coal, Iron, Gold}

    public PickUpType Type;

    public float amount = 0.0f;
    

    public override bool Interact() //pick up
    {
        throw new System.NotImplementedException();
    }
}
