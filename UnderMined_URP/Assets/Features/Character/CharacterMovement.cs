using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    private CharacterController _characterController;

    [SerializeField] private Vector3 moveVec = new Vector3(0, 0, 0);
    
    [Header("Stats")]
    [SerializeField] private float moveSpeed;

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
        _characterController.Move(moveVec);
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 inputVec = context.ReadValue<Vector2>();
        moveVec = new Vector3(inputVec.x, 0, inputVec.y) * moveSpeed * Time.deltaTime;
        
        if (context.performed)
            return;
    }

    public void Dash(InputAction.CallbackContext context)
    {
        Debug.Log("Dash");
        
        
        if (context.performed)
            return;
    }

    public void Interact(InputAction.CallbackContext context)
    {
        Debug.Log("Interact");
        
        if (context.performed)
            return;
    }

    public void Attack(InputAction.CallbackContext context)
    {
        Debug.Log("Attack");
        
        if (context.performed)
            return;
    }
    
    public void Throw(InputAction.CallbackContext context)
    {
        Debug.Log("Throw");
        
        if (context.performed)
            return;
    }
}
