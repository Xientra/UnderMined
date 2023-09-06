using Features.Cave.Chunk_System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

[SelectionBase]
public class DrillController : MonoBehaviour
{
    [SerializeField]
    public bool isRunning = false;

    public float timeRemaining = 30f;

    public float coalToTimeRatio = 10f;

    public float speed = 2f;
    public float acceleration = 0.05f;
    public float steerSpeed = 25f;

    [Space(5)]
    
    public UnityEvent die;

    [Header("Mining:")]
    
    public float miningSize = 2f;
    public float miningStrength = 0.8f;
    public GameObject miningPoint;
    
    public float mineDelay = 0.5f;

    private float _mineTimestamp = 0f;
    

    [Header("Effects:")]
    
    public VisualEffect drillVfx;

    public void StartMoving()
    {
        isRunning = true;
        drillVfx.Play();
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
            ChunkManager.instance.MineWall(miningPoint.transform.position, miningSize, miningStrength);
            _mineTimestamp = Time.time + mineDelay;
        }
    }

    public void AddCoal(float amount)
    {
        timeRemaining += amount * coalToTimeRatio;
    }

    public void StealCoal(float amount)
    {
        timeRemaining -= amount * coalToTimeRatio;
    }

    private void OnDie()
    {
        isRunning = false;
        drillVfx.Stop();
        die.Invoke();
    }

    public void Steer(float dir)
    {
        transform.Rotate(new Vector3(0, dir * steerSpeed * Time.deltaTime, 0));
    }
}