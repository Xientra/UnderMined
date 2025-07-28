using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Interactable : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<CharacterInputController>().currentInteractable = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<CharacterInputController>().currentInteractable == this)
            {
                other.GetComponent<CharacterInputController>().currentInteractable = null;
            }
        }
    }

    public abstract void Interact( CharacterInputController player);
}
