using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour, IHittable
{
    public DrillController target;

    public int health = 2;
    
    public float digSpeed = 4f;
    public float moveSpeed = 2f;
    public float turnSpeed = 0.1f;

    public bool digging = true;

    public float aimAccuracy = 15f;

    public float killDistance = 100f;
    
    public float attackRange = 2f;

    public float attackCooldown = 1f;
    private float _timeTillAttack = 1f;

    public float coalSteal = 0.5f;

    public bool dead = false;
    
    [Space(5)]
    
    public GameObject graphic;
    public VisualEffect diggingVfx;

    [Header("Effects:")]
    public Animator animator;
    
    [Space(5)]
    
    public GameObject emergeVFX;

    private Vector3 _turnVelocity;

    private void Start()
    {
        SetRandomDirection();
    }

    private void Update()
    {
        if (dead)
            return;
        
        Vector3 toTarget = target.transform.position - transform.position;
        if (toTarget.sqrMagnitude < attackRange * attackRange)
        {
            _timeTillAttack -= Time.deltaTime;
            if (_timeTillAttack <= 0)
            {
                AttackTarget();
                _timeTillAttack = attackCooldown;
            }
            animator.SetBool("Movement/isWalking", false);
        }
        else
        {
            _timeTillAttack = attackCooldown;
            Move();
            if (digging == false)
                animator.SetBool("Movement/isWalking", true);
        }
        
        if (toTarget.sqrMagnitude > killDistance * killDistance)
            Destroy(this.gameObject);
    }

    private void Move()
    {
        float speed = digging ? digSpeed : moveSpeed;

        if (digging == false)
        {
            // turn towards target
            Vector3 toTarget = target.transform.position - transform.position;
            transform.forward = Vector3.SmoothDamp(transform.forward, toTarget, ref _turnVelocity, turnSpeed);
            
            animator.SetFloat("Movement/walkSpeed", speed * Time.deltaTime);
        }

        transform.position += transform.forward * (speed * Time.deltaTime);
    }

    private void SetRandomDirection()
    {
        Vector2 rndCircle = Random.insideUnitCircle;
        Vector3 targetPoint = target.transform.position + new Vector3(rndCircle.x, 0, rndCircle.y) * aimAccuracy;
        Vector3 direction = targetPoint - transform.position;

        transform.forward = direction;
    }

    private void OnCollisionExit(Collision other)
    {
        if (digging)
            Emerge(Vector3.up);
    }

    private void Emerge(Vector3 normal)
    {
        digging = false;
        diggingVfx.Stop();
        Instantiate(emergeVFX, transform.position, Quaternion.identity).transform.up = normal;
        graphic.SetActive(true);
    }

    private void AttackTarget()
    {
        animator.SetTrigger("Action/Attack");
        target.StealCoal(coalSteal);
    }

    private void OnDrawGizmos()
    {
        if (digging)
            Debug.DrawLine(transform.position, transform.position + transform.forward * 100, Color.red);

        Vector3 pos = target != null ? target.transform.position : transform.position;

        Gizmos.DrawWireSphere(pos, aimAccuracy);
    }

    public void GetHit(int amount)
    {
        health -= amount;
        if (health < 0)
            Die();
    }

    private void Die()
    {
        // TODO: die vfx + die animation
        dead = true;
        animator.SetTrigger("Action/Die");
        
        Destroy(this.gameObject, 2f);
    }
}