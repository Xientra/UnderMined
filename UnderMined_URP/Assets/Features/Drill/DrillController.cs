using System;
using System.Collections.Generic;
using Features.Cave.Chunk_System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

[SelectionBase]
public class DrillController : MonoBehaviour
{
    [SerializeField]
    public bool isRunning = false;

    public float timeRemaining = 0;
    public float maxTimeAmount = 60f;
    public float coalToTimeRatio = 10f;

    [Header("Movement:")]
    
    public float speed = 2f;
    public float acceleration = 0.05f;
    public float steerSpeed = 25f;

    [Space(5)]
    
    public UnityEvent die;

    [Header("Mining:")]
    
    public Vector2 miningSizeMinMax = new Vector2(4f, 4.5f);
    public float miningStrength = 0.8f;
    public GameObject miningPoint;
    
    public float mineDelay = 0.5f;

    private float _mineTimestamp = 0f;
    

    public Dictionary<WallType, float> collectedOre = new Dictionary<WallType, float>();
    public Dictionary<WallType, float> totalCollectedOre = new Dictionary<WallType, float>();

    [Header("Effects:")]
    
    public VisualEffect drillVfx;

    public Animator animator;

    public VisualEffect explodeVfx;

    private void Awake()
    {
        // for things that are shown on UI
        collectedOre.TryAdd(WallType.Gold, 0);
        totalCollectedOre.TryAdd(WallType.Gold, 0);
        collectedOre.TryAdd(WallType.Booster, 0);
        totalCollectedOre.TryAdd(WallType.Booster, 0);
    }

    public float GetOreAmount(WallType ore)
    {
        collectedOre.TryGetValue(ore, out var amount);
        return amount;
    }

    public void StartMoving()
    {
        isRunning = true;
        drillVfx.Play();
        animator.SetBool("isMining", true);
        animator.SetBool("isDriving", true);
    }

    private void Update()
    {
        if (isRunning == false)
            return;

        Move();

        Mine();

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
            OnDie();
    }

    private void Move()
    {
        transform.position += transform.forward * (speed * Time.deltaTime);

        speed += acceleration * Time.deltaTime;
    }

    private void Mine()
    {
        if (Time.time > _mineTimestamp)
        {
            ChunkManager.instance.MineWall(miningPoint.transform.position, Random.Range(miningSizeMinMax.x, miningSizeMinMax.y), miningStrength);
            _mineTimestamp = Time.time + mineDelay;
        }
    }

    public void AddOre(WallType oreType, float amount)
    {
        if (oreType == WallType.Coal)
        {
            // start game on first coal pickup
            if (GameManager.instance.gameIsRunning == false)
                GameManager.instance.StartGame();
            
            // add time
            timeRemaining += amount * coalToTimeRatio;
            if (timeRemaining > maxTimeAmount)
                timeRemaining = maxTimeAmount;
        }
        else
        {
            collectedOre.TryAdd(oreType, 0);
            collectedOre[oreType] += amount;
        }

        
        totalCollectedOre.TryAdd(oreType, 0);
        totalCollectedOre[oreType] += amount;
    }

    public void StealCoal(float amount)
    {
        timeRemaining -= amount * coalToTimeRatio;
    }

    private void OnDie()
    {
        isRunning = false;
        animator.SetBool("isMining", false);
        animator.SetBool("isDriving", false);
        drillVfx.Stop();
        explodeVfx.Play();
        die.Invoke();
    }

    public void Steer(float dir)
    {
        transform.Rotate(new Vector3(0, dir * steerSpeed * Time.deltaTime, 0));
    }
}