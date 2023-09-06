using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public JoinManager joinManager;
    public DrillController drill;
    public EnemySpawner enemySpawner;
    
    [Header("Menu:")]
    
    public GameObject mainMenu;
    public GameObject inGameMenu;
    public TextMeshProUGUI timerLabel;
    public GameObject endScreen;

    [Space(10)]
    
    public bool gameIsRunning = false;

    public float gold = 0f;
    
    private void Start()
    {
        joinManager = FindObjectOfType<JoinManager>();
        drill = FindObjectOfType<DrillController>();
        enemySpawner = FindObjectOfType<EnemySpawner>();
        drill.die.AddListener(OnDrillDie);
    }

    private void Update()
    {
        if (gameIsRunning)
            timerLabel.text = TimeSpan.FromSeconds(drill.timeRemaining).ToString("hh':'mm':'ss");
    }

    public void Btn_StartGame()
    {
        mainMenu.SetActive(false);
        inGameMenu.SetActive(true);
        
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