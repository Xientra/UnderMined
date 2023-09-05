using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Attack : MonoBehaviour
{
    public CharacterInputController player;
    private void OnTriggerEnter(Collider other)
    {
        IHittable hit = other.GetComponent<IHittable>();
        if (hit != null)
        {
            hit.GetHit(player.damage);
        }
    }
}
