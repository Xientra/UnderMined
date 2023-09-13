using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameGUI : MonoBehaviour
{
    private GameManager _gameManager;
    
    [Header("Menu:")]

    public Image healthBar;
    public Image boosterBar;
    public TextMeshProUGUI moneyLabel;
    public TextMeshProUGUI timerLabel;
    
    private void Start()
    {
        _gameManager = GameManager.instance;
    }

    private void Update()
    {
        if (_gameManager.gameIsRunning)
        {
            //timerLabel.text = TimeSpan.FromSeconds(drill.timeRemaining).ToString("hh':'mm':'ss");

            healthBar.fillAmount = _gameManager.drillTime01;
            boosterBar.fillAmount = _gameManager.boosterAmount01;
            
            moneyLabel.text = Mathf.RoundToInt(_gameManager.GoldAmount) + "";
        }
    }
}
