using System.Collections.Generic;
using Features.Cave.Chunk_System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public JoinManager joinManager;
    public DrillController drill;
    public EnemySpawner enemySpawner;

    [Header("Run Information:")]
    
    public bool gameIsRunning = false;
    [Space(10)]
    public float goldWorth = 10000f;
    public float GoldAmount => drill.collectedOre[WallType.Gold] * goldWorth;

    [Header("Starting Zone:")]
    
    public float startingZoneSize = 15f;

    public float startTime = 30f;
    public float drillTime01 => drill.timeRemaining / drill.maxTimeAmount;
    public float boosterAmount01 => drill.remainingBoostTime / drill.timeRemaining;

    [Header("Menu:")]
    
    public GameObject mainMenu;
    public GameObject inGameMenu;
    public GameObject endScreen;
    
    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        joinManager = FindObjectOfType<JoinManager>();
        drill = FindObjectOfType<DrillController>();
        enemySpawner = FindObjectOfType<EnemySpawner>();
        drill.die.AddListener(OnDrillDie);

        ChunkManager.instance.MineWall(drill.transform.position, startingZoneSize, 0.75f);
    }

    public void Btn_StartGame()
    {
        StartGame();
    }

    public void StartGame()
    {
        mainMenu.SetActive(false);
        inGameMenu.SetActive(true);

        drill.timeRemaining = startTime;
        drill.StartMoving();
        gameIsRunning = true;
        enemySpawner.isSpawning = true;
        joinManager.EnableMining(true);
    }

    public void EndGame()
    {
        inGameMenu.SetActive(false);
        endScreen.SetActive(true);
        
        gameIsRunning = false;
        enemySpawner.isSpawning = false;
        joinManager.EnableMining(false);
    }

    public void OnDrillDie()
    {
        EndGame();
    }

    public void Btn_RestartGame()
    {
        // reload scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}