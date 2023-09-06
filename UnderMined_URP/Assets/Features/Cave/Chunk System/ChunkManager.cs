using System;
using System.Collections.Generic;
using UnityEngine;

namespace Features.Cave.Chunk_System
{
    public class ChunkManager : MonoBehaviour
    {
        public static ChunkManager instance;
        
        public const int chunkSize = 64;

        
        
        public GameObject target;

        public CaveChunk[] chunkPool = new CaveChunk[9];

        public Dictionary<Vector2Int, GridPoint[,]> caveChunkValues;
        


        public void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            Vector2Int targetGridPos = GetTargetChunkGridPosition();

            chunkPool[0].transform.position = GridToWorldPosition(targetGridPos + new Vector2Int(-1, -1));
            chunkPool[1].transform.position = GridToWorldPosition(targetGridPos + new Vector2Int(0, -1));
            chunkPool[2].transform.position = GridToWorldPosition(targetGridPos + new Vector2Int(1, -1));
            chunkPool[3].transform.position = GridToWorldPosition(targetGridPos + new Vector2Int(-1, 0));
            chunkPool[4].transform.position = GridToWorldPosition(targetGridPos + new Vector2Int(0, 0));
            chunkPool[5].transform.position = GridToWorldPosition(targetGridPos + new Vector2Int(1, 0));
            chunkPool[6].transform.position = GridToWorldPosition(targetGridPos + new Vector2Int(-1, 1));
            chunkPool[7].transform.position = GridToWorldPosition(targetGridPos + new Vector2Int(0, 1));
            chunkPool[8].transform.position = GridToWorldPosition(targetGridPos + new Vector2Int(1, 1));
        }

        private Vector2Int GetTargetChunkGridPosition()
        {
            float xPos = target.transform.position.x;
            float yPos = target.transform.position.z;

            int x = Mathf.FloorToInt(xPos / chunkSize) % chunkSize;
            int y = Mathf.FloorToInt(yPos / chunkSize) % chunkSize;

            return new Vector2Int(x, y);
        }
        
        private Vector3 GridToWorldPosition(Vector2Int gridPos)
        {
            return new Vector3(gridPos.x * chunkSize, 0,gridPos.y * chunkSize);
        }

        private void OnDestroy()
        {
            instance = null;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(GridToWorldPosition(GetTargetChunkGridPosition()), 5f);
        }
    }
}
