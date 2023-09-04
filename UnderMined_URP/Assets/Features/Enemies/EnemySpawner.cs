using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public bool isSpawning = false;

    void Start()
    {
    }

    private void Update()
    {
        if (isSpawning == false)
            return;
    }
}