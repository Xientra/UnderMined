using System;
using System.Collections;
using System.Collections.Generic;
using Features.Cave.Chunk_System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

[RequireComponent(typeof(CharacterController))]
public class CharacterInputController : MonoBehaviour
{
    private CharacterController _characterController;
    public DrillController _drillController;

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
    [SerializeField] public bool isSteeringDrill = false;
	[Space(5)]
	[SerializeField] private GameObject revivePrefab;

    [Header("Combat Variables")]
    [SerializeField] private int health = 1;
    [SerializeField] public bool isFallen = false;
    [SerializeField] public int damage = 1;
    [SerializeField] private GameObject attackCube;
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private bool attackAvailable = true;

    [Header("Mining Variables")]
    [SerializeField] private float miningStrength = 0.5f;
    [SerializeField] private float miningRadius = 1f;

    [Header("Animation")]
    
    public Animator animator;

    [Header("VFX:")]
    
    public GameObject throwVfxPrefab;

    public VisualEffect mineVfx;
    
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
        if(!isFallen)
        {
            if (!isSteeringDrill)
            {
                _characterController.Move(moveVec * (moveSpeed * Time.deltaTime));

                if (moveVec != Vector3.zero)
                    transform.forward = moveVec;
                
                
                
            }
            else
            {
                _drillController.Steer(moveVec.x);
            }
            
            
            animator.SetBool("Movement/isWalking", (moveVec * (moveSpeed * Time.deltaTime)).magnitude > 0.1f);
            animator.SetFloat("Movement/walkSpeed", (moveVec * (moveSpeed * Time.deltaTime)).magnitude);
        }
        
        if (pickUp != null)
            animator.SetBool("Action/isCarrying", true);
        
        
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
                if (currentInteractable)    //interact
                {
                    Type interactableType = currentInteractable.GetType();
                    switch (currentInteractable)
                    {
                        case PickUp:
                            break;
                        
                        case Refill:
                            animator.SetTrigger("Action/Interact");
                            break;

                        case SteeringWheel:
                            animator.SetTrigger("Action/Interact");
                            break;
                        
                        case Revive:
                            animator.SetTrigger("Action/Interact");
                            break;

                        case StartGame:
                            animator.SetTrigger("Action/Interact");
                            break;
                        
                        default:
                            //todo: explode
                            break;
                    }
                    currentInteractable.Interact(this);
                }
                else 
                {
                    if (pickUp) //drop carried stuff
                    {
                        pickUp.transform.SetParent(null);
                        pickUp.rb.useGravity = true;
                        pickUp.rb.constraints = RigidbodyConstraints.None;
                        pickUp.col.enabled = true;
                        pickUp = null;
                        currentInteractable = null;
                        //todo: default back to idle/walking anim
                    }
                    else if (isSteeringDrill) //get out of drill
                    {
                        isSteeringDrill = false;
                        this.gameObject.transform.SetParent(null);
                        //todo: default back to idle/walking anim       
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
                    animator.SetTrigger("Action/Attack");
                    mineVfx.Play();
                    MiningResult mr = ChunkManager.instance.MineWall(attackCube.transform.position, miningRadius, miningStrength);
                    SpawnPickups(mr);
                    StartCoroutine(attackHandling());
                    StartCoroutine(attackCooldownHandling(attackCooldown));
                }
            }
        }

        
        [Header("Ore Pickup Spawn:")] // TODO: put this somewhere else at some point
        public float minOreAmount = 0.5f;
        public PickUp goldPickup;
        public PickUp coalPickup;

        private MiningResult storedOre = new MiningResult();
        
        private void SpawnPickups(MiningResult mr)
        {
            storedOre.Add(mr);

            if (storedOre.goldAmount > minOreAmount)
            {
                Instantiate(goldPickup.gameObject, transform.position, Quaternion.identity)
                    .GetComponent<PickUp>().amount = mr.goldAmount;
                storedOre.goldAmount -= minOreAmount;
            }

            if (storedOre.coalAmount > minOreAmount)
            {
                Instantiate(coalPickup.gameObject, transform.position, Quaternion.identity)
                    .GetComponent<PickUp>().amount = mr.coalAmount;
                storedOre.coalAmount -= minOreAmount;
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

                Instantiate(throwVfxPrefab, pickUp.transform.position, pickUp.transform.rotation, pickUp.transform);
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

    public void onDeath()
    {
        this.isFallen = true;
        Instantiate(revivePrefab).GetComponent<Revive>().fallenplayer = this;
        animator.SetTrigger("Action/Die");
    }
}
