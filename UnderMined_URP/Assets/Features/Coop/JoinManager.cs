using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class JoinManager : MonoBehaviour
{
    public PlayerInput[] players = new PlayerInput[4];
    private int _playerCount = 0;

    public float spawnRadius = 6f;

    public CinemachineTargetGroup targetGroup;
    
    void Start()
    {
    }

    void Update()
    {
    }

    private Vector3 GetPlayerSpawnPosition()
    {
        Vector2 rndCircleEdge = Random.insideUnitCircle;
        return GameManager.instance.drill.transform.position + new Vector3(rndCircleEdge.x, 0, rndCircleEdge.y) * spawnRadius;
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        players[_playerCount++] = playerInput;

        targetGroup.AddMember(playerInput.transform, 1, 1);
        StartCoroutine(SetPlayerPosAfter1Frame(playerInput.gameObject));
    }

    private IEnumerator SetPlayerPosAfter1Frame(GameObject go)
    {
        yield return null;
        go.transform.position = GetPlayerSpawnPosition();
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        // meh i'll do it when i have the time
    }

    public void EnableMining(bool value)
    {
        
        if (value)
        {
            for (int i = 0; i < players.Length; i++)
            {
                //players[i].
            }
        }
        else
        {
            for (int i = 0; i < players.Length; i++)
            {
                CharacterInputController player = players[i].GetComponent<CharacterInputController>();
                player.isSteeringDrill = false;
                player.transform.parent = null;
            }
        }
        
    }
}