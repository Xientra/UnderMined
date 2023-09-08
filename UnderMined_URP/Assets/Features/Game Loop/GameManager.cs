using System;
using System.Collections;
using System.Collections.Generic;
using Features.Cave.Chunk_System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public JoinManager joinManager;
    public DrillController drill;
    public EnemySpawner enemySpawner;

    [Header("Starting Zone:")]
    
    public float startingZoneSize = 15f;

    public float startTime = 30f;
    
    [Header("Menu:")]
    
    public GameObject mainMenu;
    public GameObject inGameMenu;
    public TextMeshProUGUI timerLabel;
    public GameObject endScreen;

    [Space(5)]
    
    public Image healthBar;

    public TextMeshProUGUI moneyLabel;
    public float goldWorth = 10000f;

    [Space(10)]
    
    public bool gameIsRunning = false;

    public float gold = 0f;

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

    private void Update()
    {
        if (gameIsRunning)
        {
            //timerLabel.text = TimeSpan.FromSeconds(drill.timeRemaining).ToString("hh':'mm':'ss");

            healthBar.fillAmount = drill.timeRemaining / drill.maxTimeAmount;
            
            moneyLabel.text = gold * goldWorth + "";
        }
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