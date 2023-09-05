using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class CharacterController : MonoBehaviour
{
    private CharacterController _characterController;


    [Header("Move Stats")]
    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private Vector3 moveVec = Vector3.zero;
    [SerializeField] private bool moveAvailable = true;
    
    [Header("Dash Stats")]
    [SerializeField] private float dashPower = 3.0f;
    [SerializeField] private float dashCD = 2.0f;
    [SerializeField] private float dashDuration = 1.0f;
    [SerializeField] private bool dashAvailable = true;
    
    [Header("Throw Stats")]
    [SerializeField] private float throwPower = 10.0f;
    [SerializeField]

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
                Debug.Log("Interact");
            }
            return;
        }

        public void Attack(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                Debug.Log("Attack");
            }
            return;
        }
        
        public void Throw(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                Debug.Log("Throw");
            }
            return;
        }
        
    #endregion

   

    
}
