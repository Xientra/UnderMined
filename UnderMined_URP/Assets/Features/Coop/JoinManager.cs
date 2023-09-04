using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JoinManager : MonoBehaviour
{
    public PlayerInput[] players = new PlayerInput[4];
    private int _playerCount = 0;
    
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        players[_playerCount++] = playerInput;
    }
    
    public void OnPlayerLeft(PlayerInput playerInput)
    {
        // meh i'll do it when i have the time
    }

    public void EnableMining()
    {
        for (int i = 0; i < players.Length; i++)
        {
            //players[i].
        }
    }
}
