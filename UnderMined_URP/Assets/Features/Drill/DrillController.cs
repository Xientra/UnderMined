using System;
using System.Collections;
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
    

    public OreCollection collectedOre = new OreCollection();
    public OreCollection totalCollectedOre = new OreCollection();

    [Header("Boost:")]
    
    public DrillStates drillState = DrillStates.Off;
    public enum DrillStates  { Off, Running, BoostReady, Boosting, Destroyed, Cutscene }

    public bool isBoostReady = false;
    public bool inBoostMode = false;

    public float oreNeededForBoostMode = 300f;

    [Space(5)]
    
    public float boostTime = 30f;
    public float remainingBoostTime = 30f;

    public float boostSpeed = 10f;
    public float boostMineCooldown = 0.3f;
    
    [Header("Target Zone:")]
    
    public TargetZone targetZonePrefab;

    public float targetZoneDistance = 100f;

    public float downwardsAngle = 30f;
    public float timeBeforeSceneReload = 6f;
    public float drillDownSpeed = 50f;
    
    public EventHandler advanceToNextStage;

    [Header("Effects:")]
    
    public VisualEffect drillVfx;
    public VisualEffect drillBoostVfx;
    public VisualEffect boostReadyVfx;

    public Animator animator;

    public VisualEffect explodeVfx;

    public void StartMoving()
    {
        isRunning = true;
        drillVfx.Play();
        animator.SetBool("isMining", true);
        animator.SetBool("isDriving", true);

        AddOre(WallType.Booster, 300); // DEBUG
    }

    private void Update()
    {
        if (isRunning == false || isBoostReady)
            return;

        Move();

        Mine();


        if (inBoostMode == false)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0)
                OnDie();
        }
    }

    private void Move()
    {
        float s = inBoostMode ? boostSpeed : speed;
        
        transform.position += transform.forward * (s * Time.deltaTime);

        speed += acceleration * Time.deltaTime;
    }

    private void Mine()
    {
        if (Time.time > _mineTimestamp)
        {
            ChunkManager.instance.MineWall(miningPoint.transform.position, Random.Range(miningSizeMinMax.x, miningSizeMinMax.y), miningStrength);
            _mineTimestamp = Time.time + (inBoostMode ? boostMineCooldown : mineDelay);
        }
    }

    private void SetBoostReady()
    {
        isBoostReady = true;
        boostReadyVfx.Play();
    }

    public void SetBoostMode()
    {
        inBoostMode = true;
        isBoostReady = false;

        drillBoostVfx.Play();
        boostReadyVfx.Stop();
        
        // create target zone
        Vector2 randomInCircle = Random.insideUnitCircle;
        Vector3 targetZonePosition = transform.position + new Vector3(randomInCircle.x, 0, randomInCircle.y) * targetZoneDistance;
        Instantiate(targetZonePrefab.gameObject, targetZonePosition, Quaternion.identity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TargetZone"))
            StartCoroutine(AdvancingToNextStage());
    }

    public IEnumerator AdvancingToNextStage()
    {
        Vector3 localEuler = transform.localRotation.eulerAngles;
        localEuler.x = downwardsAngle;
        transform.localRotation = Quaternion.Euler(localEuler);

        yield return new WaitForSeconds(timeBeforeSceneReload);
        
        advanceToNextStage?.Invoke(this, EventArgs.Empty);
        Debug.Log("LOAD NEW SCENE HERE");
    }

    public void AddOre(WallType oreType, float amount)
    {
        switch (oreType)
        {
            case WallType.Coal:
                // start game on first coal pickup
                if (GameManager.instance.gameIsRunning == false)
                    GameManager.instance.StartGame();
            
                // add time instead of time
                timeRemaining += amount * coalToTimeRatio;
                if (timeRemaining > maxTimeAmount)
                    timeRemaining = maxTimeAmount;
                break;
            
            case WallType.Booster:
                collectedOre.AddOre(oreType, amount);
                if (collectedOre[WallType.Booster] > oreNeededForBoostMode) // clamp
                {
                    collectedOre[WallType.Booster] = oreNeededForBoostMode;
                    if (isBoostReady == false)
                        SetBoostReady();
                }
                break;
            
            default:
                collectedOre.AddOre(oreType, amount);
                break;
        }

        totalCollectedOre.AddOre(oreType, amount);
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