using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Features.Cave.Chunk_System
{
    public class ChunkManager : MonoBehaviour
    {
        public static ChunkManager instance;

        public const int ChunkSize = 64;

        public const float CellSize = 1.0f;

        private Dictionary<Vector2Int, GridPoint[,]> _caveChunkValues = new Dictionary<Vector2Int, GridPoint[,]>();

        public float noiseScale = 0.2f;


        public Vector2Int currentCenterChunkIndex;

        public GameObject target;

        public CaveChunk[] chunkPool = new CaveChunk[9];


        [Header("Ore Generation:")]
        
        public float[] oreFrequencies = new[] { 0.1f, 0.5f };
        public float oreAmount = 0.5f;
        private Vector3 randomOffsetPerRun = Vector3.zero;
        
        public void Awake()
        {
            instance = this;
            _caveChunkValues = new Dictionary<Vector2Int, GridPoint[,]>();

            randomOffsetPerRun = new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100));
        }

        private void Start()
        {
            currentCenterChunkIndex = GetTargetChunkGridPosition();
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

        private GridPoint[,] GetValueFieldAtGridPos(Vector2Int gridPos)
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

        private GridPoint.WallType GetWallType(Vector3 position)
        {
            bool isCoal = OreGeneratorFunction(position + new Vector3(0, 0, 100));
            if (isCoal)
                return GridPoint.WallType.Coal;
            
            bool isGold = OreGeneratorFunction(position + new Vector3(100, 0, 0));
            if (isGold)
                return GridPoint.WallType.Gold;

            return GridPoint.WallType.Stone;
        }

        private bool OreGeneratorFunction(Vector3 position)
        {
            float oreValue = 1f;
            
            for (int i = 0; i < oreFrequencies.Length; i++)
                oreValue *= Mathf.PerlinNoise(position.x * oreFrequencies[i], position.z * oreFrequencies[i]);

            return oreValue > oreAmount;
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
                Vector3 gridPointPos = gridOrigin + new Vector3(x * CellSize, 0, y * CellSize);
                //float value = Mathf.PerlinNoise(gridPointPos.x * noiseScale, gridPointPos.z * noiseScale);
                float value = 1f;
                GridPoint p = new GridPoint(gridPointPos, value);
                p.wallType = GetWallType(gridPointPos);
                newField[x, y] = p;
            }

            return newField;
        }

        private Vector2Int GetTargetChunkGridPosition()
        {
            return WorldToGridPosition(target.transform.position);
        }

        private Vector2Int WorldToGridPosition(Vector3 worldPos)
        {
            return new Vector2Int(
                Mathf.FloorToInt(worldPos.x / ChunkSize) % ChunkSize, 
                Mathf.FloorToInt(worldPos.z / ChunkSize) % ChunkSize);
        }
        
        private Vector3 GridToWorldPosition(Vector2Int gridPos)
        {
            return new Vector3(gridPos.x * ChunkSize, 0, gridPos.y * ChunkSize);
        }

        /// <summary>
        /// Removes part of the  wall at the given position.<br/>
        /// point is the world position of mining, radius the radius and strength is between 0 and 1 the amount subtracted from the wall.
        /// </summary>
        public MiningResult MineWall(Vector3 point, float radius, float strength)
        {
            // TODO: make this cross chunks
            
            Vector2Int chunkGridPos = WorldToGridPosition(point);

            GridPoint[,] valueField = GetValueFieldAtGridPos(chunkGridPos);
            
            // TODO: make this cross chunks
            MiningResult mr = new MiningResult();

            // TODO: maybe optimize 
            
            for (int y = 0; y < ChunkSize; y++)
                for (int x = 0; x < ChunkSize; x++)
                if ((valueField[x, y].pos - point).sqrMagnitude < radius * radius)
                {
                    float removeAmount = Mathf.Min(valueField[x, y].value, strength);
                    valueField[x, y].value -= removeAmount;
                    if (valueField[x, y].wallType == GridPoint.WallType.Stone)
                        mr.stoneAmount += removeAmount;
                    else if (valueField[x, y].wallType == GridPoint.WallType.Coal)
                        mr.coalAmount += removeAmount;
                    else if (valueField[x, y].wallType == GridPoint.WallType.Gold)
                        mr.goldAmount += removeAmount;
                }

            return mr;
        }

        private void OnDestroy()
        {
            instance = null;
        }

        private void OnDrawGizmosSelected()
        {
            if (target == null)
                return;
            
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(GridToWorldPosition(GetTargetChunkGridPosition()), 5f);
        }
    }

    [System.Serializable]
    public class MiningResult
    {
        public float stoneAmount = 0f;
        public float coalAmount = 0f;
        public float goldAmount = 0f;
    }
}