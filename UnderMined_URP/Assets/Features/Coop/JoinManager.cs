using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class JoinManager : MonoBehaviour
{
    public PlayerInput[] players = new PlayerInput[4];
    private int _playerCount = 0;

    public float spawnRadius = 2f;

    public CinemachineTargetGroup targetGroup;
    
    void Start()
    {
    }

    void Update()
    {
    }

    private Vector3 GetPlayerSpawnPosition()
    {
        Vector2 rndCircleEdge = Random.insideUnitCircle.normalized;
        Vector3 newPos = GameManager.instance.drill.transform.position + new Vector3(rndCircleEdge.x, 1, rndCircleEdge.y) * spawnRadius;
        return newPos;
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        players[_playerCount++] = playerInput;

        CharacterController cic = playerInput.GetComponent<CharacterController>();
        
        targetGroup.AddMember(playerInput.transform, 1, 1);
    }


    public void OnPlayerLeft(PlayerInput playerInput)
    {
        // meh i'll do it when i have the time
    }

    public void EnableMining(bool value)
    {
        
        for (int i = 0; i < players.Length; i++)
            players[i].GetComponent<CharacterInputController>().canMine = value;
        
        if (value)
        {
            
        }
        else
        {
            for (int i = 0; i < players.Length; i++)
                if (players[i] != null)
                {
                    CharacterInputController player = players[i].GetComponent<CharacterInputController>();
                    player.isSteeringDrill = false;
                    player.transform.parent = null;
                }
        }
        
    }
}