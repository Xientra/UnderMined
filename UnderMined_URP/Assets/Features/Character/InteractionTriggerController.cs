using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
public class InteractionTriggerController : MonoBehaviour
{
    [SerializeField] private CharacterInputController player;

    
    private void OnTriggerEnter(Collider other)
    {
        Interactable interactable = other.gameObject.GetComponent<Interactable>();
        if (interactable)
        {
            if (interactable.Interact( player))
            {
                
            }
        }
            
    }
}
