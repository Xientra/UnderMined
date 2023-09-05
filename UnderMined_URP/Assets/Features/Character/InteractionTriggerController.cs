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
            player.currentInteractable = interactable;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        Interactable interactable = other.gameObject.GetComponent<Interactable>();
        if (interactable)
        {
            if (player.currentInteractable == interactable)
                player.currentInteractable = null;
        }
    }
}
