using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class CharacterInputController : MonoBehaviour
{
    private CharacterController _characterController;

    [Header("Move Variables")]
    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private Vector3 moveVec = Vector3.zero;
    [SerializeField] private bool moveAvailable = true;
    
    [Header("Dash Variables")]
    [SerializeField] private float dashPower = 3.0f;
    [SerializeField] private float dashCD = 2.0f;
    [SerializeField] private float dashDuration = 1.0f;
    [SerializeField] private bool dashAvailable = true;
    
    [Header("Carry and Throw Variables")]
    [SerializeField] private float throwPower = 10.0f;
    [SerializeField] public float maxCarryAmount = 5.0f;
    [SerializeField] public PickUp pickUp;
    
    [Header("Interaction Variables")]
    [SerializeField] private BoxCollider triggerArea;
    [SerializeField] public Interactable currentInteractable;

    [Header("Combat Variables")]
    [SerializeField] private int health = 1;
    [SerializeField] public int damage = 1;
    [SerializeField] private GameObject attackCube;
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private bool attackAvailable = true;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _characterController.Move(moveVec * (moveSpeed * Time.deltaTime));

        if(moveVec != Vector3.zero)
            transform.forward = moveVec;
    }
    

    #region Movement

        public void Move(InputAction.CallbackContext context)
            {
                if(moveAvailable)
                {
                    Vector2 inputVec = context.ReadValue<Vector2>();
                    moveVec = new Vector3(inputVec.x, 0, inputVec.y).normalized;
                }
            }
        
            public void Dash(InputAction.CallbackContext context)
            {
                if (context.started)
                {
                    if (dashAvailable)
                    {
                        StartCoroutine(Movement(dashDuration));
                        StartCoroutine(Cooldown(dashCD));
                    }
                }
            }
            
            IEnumerator Movement(float duration)
            {
                moveAvailable = false;
        
                if (moveVec != Vector3.zero)
                {
                    moveVec = moveVec * dashPower;
                    yield return new WaitForSeconds(duration);
                    moveAvailable = true;
                    moveVec = moveVec / dashPower;
                }
                else
                {
                    moveVec = gameObject.transform.forward * dashPower;
                    yield return new WaitForSeconds(duration);
                    moveAvailable = true;
                    moveVec = Vector3.zero;
                }
            }
    
            IEnumerator Cooldown(float duration)
            {
                dashAvailable = false;
                yield return new WaitForSeconds(duration);
                dashAvailable = true;
            }

    #endregion

    #region Actions
    
        public void Interact(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                if (currentInteractable)
                {
                    currentInteractable.Interact(this);
                }
                else 
                {
                    if (pickUp)
                    {
                        pickUp.transform.SetParent(null);
                        pickUp.rb.useGravity = true;
                        pickUp.rb.constraints = RigidbodyConstraints.None;
                        pickUp.col.enabled = true;
                        pickUp = null;
                        currentInteractable = null;
                    }
                }
            }
        }

        public void Attack(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                if(attackAvailable)
                {
                    StartCoroutine(attackHandling());
                    StartCoroutine(attackCooldownHandling(attackCooldown));
                }
            }
        }
        
        public void Throw(InputAction.CallbackContext context)
        {
            if (pickUp)
            {
                pickUp.transform.SetParent(null);
                pickUp.rb.useGravity = true;
                pickUp.rb.constraints = RigidbodyConstraints.None;
                pickUp.col.enabled = true;
                pickUp.rb.AddForce((moveVec + (transform.forward + Vector3.up / 2)) * throwPower);
                pickUp = null;
                currentInteractable = null;
            }
        }

        IEnumerator attackHandling()
        {
            attackCube.SetActive(true);
            yield return new WaitForSeconds(.1f);
            attackCube.SetActive(false);
        }
        
        IEnumerator attackCooldownHandling(float duration)
        {
            attackAvailable = false;
            yield return new WaitForSeconds(duration);
            attackAvailable = true;
        }
        
    #endregion

   

    
}
