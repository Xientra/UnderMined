using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenHatchet : MonoBehaviour
{
    public Animator animator;

    public bool left;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (left)
                animator.SetBool("openLeft", true);
            else
                animator.SetBool("openRight", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (left)
                animator.SetBool("openLeft", false);
            else
                animator.SetBool("openRight", false);
        }
    }
}
