using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public DrillController target;

    public float drillSpeed = 4f;
    public float moveSpeed = 2f;
    public float turnSpeed = 0.1f;

    public bool drilling = true;

    [Header("Effects:")]
    
    public GameObject emergeVFX;
    public GameObject spawnVFX;

    private Vector3 _turnVelocity;
    
    private void Start()
    {
        //target = FindObjectOfType<DrillController>();

        //Instantiate(spawnVFX, transform.position, Quaternion.identity);
    }
    
    private void Update()
    {
        float speed = drilling ? drillSpeed : moveSpeed;

        if (drilling == false)
        {
            // turn towards target
            Vector3 toTarget = target.transform.position - transform.position;
            transform.forward = Vector3.SmoothDamp(transform.forward, toTarget, ref _turnVelocity, turnSpeed);
        }

        transform.position += transform.forward * (speed * Time.deltaTime);
    }

    private void SetDirection()
    {
        
        
    }

    private void OnCollisionExit(Collision other)
    {
        Emerge();
    }

    private void Emerge()
    {
        drilling = false;
        //Instantiate(emergeVFX, transform.position, Quaternion.identity);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawSphere(transform.position, 1f);
    }
}
