using System;
using System.Collections.Generic;
using UnityEngine;

namespace Features.Cave.Chunk_System
{
    public class ChunkManager : MonoBehaviour
    {
        public static ChunkManager instance;

        public const int ChunkSize = 64;

        public const float CellSize = 1.0f;


        public float noiseScale = 0.2f;
        

        public GameObject target;

        public CaveChunk[] chunkPool = new CaveChunk[9];

        private Dictionary<Vector2Int, GridPoint[,]> _caveChunkValues = new Dictionary<Vector2Int, GridPoint[,]>();


        public void Awake()
        {
            instance = this;
            _caveChunkValues = new Dictionary<Vector2Int, GridPoint[,]>();
        }

        private void Start()
        {
        }

        private void Update()
        {
            Vector2Int targetGridPos = GetTargetChunkGridPosition();

            SetChunkValues(0, targetGridPos + new Vector2Int(-1, -1));
            SetChunkValues(1, targetGridPos + new Vector2Int(0, -1));
            SetChunkValues(2, targetGridPos + new Vector2Int(1, -1));
            SetChunkValues(3, targetGridPos + new Vector2Int(-1, 0));
            SetChunkValues(4, targetGridPos + new Vector2Int(0, 0));
            SetChunkValues(5, targetGridPos + new Vector2Int(1, 0));
            SetChunkValues(6, targetGridPos + new Vector2Int(-1, 1));
            SetChunkValues(7, targetGridPos + new Vector2Int(0, 1));
            SetChunkValues(8, targetGridPos + new Vector2Int(1, 1));
        }

        private void SetChunkValues(int chunkIndex, Vector2Int gridPos)
        {
            chunkPool[chunkIndex].transform.position = GridToWorldPosition(gridPos);
            chunkPool[chunkIndex].SetChunkValueField(GetValueFieldAtGridPos(gridPos));
        }

        public GridPoint[,] GetValueFieldAtGridPos(Vector2Int gridPos)
        {
            if (_caveChunkValues.TryGetValue(gridPos, out var valueField))
                return valueField;
            else
            {
                GridPoint[,] newField = CreateValueField(GridToWorldPosition(gridPos));
                _caveChunkValues.Add(gridPos, newField);
                return newField;
            }
        }


        /// <summary>
        /// Creates a ValueField at the given position and returns it.
        /// </summary>
        /// <param name="gridOrigin"> is world space pos at the bottom left of the grid</param>
        private GridPoint[,] CreateValueField(Vector3 gridOrigin)
        {
            int size = ChunkSize;
            GridPoint[,] newField = new GridPoint[size, size];

            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    Vector3 gridPointPos = gridOrigin + new Vector3(x * CellSize , 0, y * CellSize);
                    float value = Mathf.PerlinNoise(gridPointPos.x * noiseScale, gridPointPos.z * noiseScale);
                    GridPoint p = new GridPoint(gridPointPos, value);
                    newField[x, y] = p;
                }

            return newField;
        }

        private Vector2Int GetTargetChunkGridPosition()
        {
            float xPos = target.transform.position.x;
            float yPos = target.transform.position.z;

            int x = Mathf.FloorToInt(xPos / ChunkSize) % ChunkSize;
            int y = Mathf.FloorToInt(yPos / ChunkSize) % ChunkSize;

            return new Vector2Int(x, y);
        }

        private Vector3 GridToWorldPosition(Vector2Int gridPos)
        {
            return new Vector3(gridPos.x * ChunkSize, 0, gridPos.y * ChunkSize);
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