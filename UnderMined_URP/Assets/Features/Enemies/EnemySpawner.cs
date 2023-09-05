using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public bool isSpawning = false;

    [Space(10)]
    
    public Enemy enemyPrefab;

    public DrillController spawnTarget;

    private Camera _camera;
    
    public Vector2 continuousSpawnDelay = new Vector2(4f, 8f);
    private float spawnDelayTimestamp = 0f;

    private void Awake()
    {
        spawnDelayTimestamp = Random.Range(continuousSpawnDelay.x, continuousSpawnDelay.y);
        
    }

    private void Start()
    {
        _camera = Camera.main;

        spawnTarget = FindObjectOfType<DrillController>();
    }

    private void Update()
    {
        if (isSpawning == false)
            return;

        if (Time.time > spawnDelayTimestamp)
            SpawnEnemy();
    }

    private float GetMinRadius()
    {
        Vector3 targetPos = spawnTarget.transform.position;

        GetXYPlanePointFromCameraCorners(0, 0);

        float distance1 = (targetPos - GetXYPlanePointFromCameraCorners(0, Screen.height)).magnitude;
        float distance2 = (targetPos - GetXYPlanePointFromCameraCorners(Screen.width, Screen.height)).magnitude;
        
        return Mathf.Max(distance1, distance2);
    }

    private Vector3 GetSpawnPosition()
    {
        Vector3 drillPosition = Vector3.zero;

        Vector2 rndOnCircle = Random.insideUnitCircle.normalized * GetMinRadius();
        
        return drillPosition + new Vector3(rndOnCircle.x, 0, rndOnCircle.y);
    }

    private void SpawnEnemy()
    {
        Vector3 spawnPos = GetSpawnPosition();
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity).GetComponent<Enemy>().target = spawnTarget;
        
        spawnDelayTimestamp = Random.Range(continuousSpawnDelay.x, continuousSpawnDelay.y);
    }

    private Vector3 GetXYPlanePointFromCameraCorners(int x, int y) => GetXYPlanePointFromCameraCorners(new Vector2(x, y));
    private Vector3 GetXYPlanePointFromCameraCorners(Vector2 screenPoint)
    {
        Plane xyPlane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = _camera.ScreenPointToRay(screenPoint);

        float enter = 0f;
        xyPlane.Raycast(ray, out enter);

        return ray.GetPoint(enter);
    }
}