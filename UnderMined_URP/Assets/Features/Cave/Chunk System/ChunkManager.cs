using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Features.Cave.Chunk_System
{
    public class ChunkManager : MonoBehaviour
    {
        public static ChunkManager instance;

        public const int ChunkSize = 64;

        public const float CellSize = 1.0f;

        public const float IsoValue = 0.5f;

        public const float WallHeight = 3f;

        public AnimationCurve miningFalloff = AnimationCurve.EaseInOut(0, 1, 0, 0);

        private class ChunkInfo
        {
            public readonly GridPoint[,] valueField;
            public readonly Dictionary<Vector3, int> gridPoints;

            public ChunkInfo(GridPoint[,] valueField, Dictionary<Vector3, int> gridPoints)
            {
                this.valueField = valueField;
                this.gridPoints = gridPoints;
            }
        }

        private Dictionary<Vector2Int, ChunkInfo> _caveChunkValues = new Dictionary<Vector2Int, ChunkInfo>();

        public float noiseScale = 0.2f;


        [FormerlySerializedAs("currentCenterChunkIndex")]
        public Vector2Int oldCenterChunkIndex;

        public GameObject target;

        public CaveChunk[] chunkPool = new CaveChunk[9];


        [Header("Ore Generation:")] public float[] oreFrequencies = new[] { 0.1f, 0.5f };
        public float oreAmount = 0.5f;
        private Vector3 randomOffsetPerRun = Vector3.zero;

        public void Awake()
        {
            instance = this;

            randomOffsetPerRun = new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100));
        }

        private void Start()
        {
            oldCenterChunkIndex = GetTargetChunkGridPosition();
            UpdateChunks(oldCenterChunkIndex);
        }

        private void Update()
        {
            Vector2Int targetGridPos = GetTargetChunkGridPosition();

            if (oldCenterChunkIndex != targetGridPos)
                UpdateChunks(targetGridPos);
        }

        private void UpdateChunks(Vector2Int targetGridPos)
        {
            // set chunks free that are too far away
            for (int i = 0; i < chunkPool.Length; i++)
                if (CheckChunkReplaceable(WorldToChunkCoord(chunkPool[i].transform.position), targetGridPos))
                    chunkPool[i].canBeReplaced = true;

            // put far away chunks to near position

            // hard settings chunks for now
            SetChunkValues(0, targetGridPos + new Vector2Int(-1, -1));
            SetChunkValues(1, targetGridPos + new Vector2Int(0, -1));
            SetChunkValues(2, targetGridPos + new Vector2Int(1, -1));
            SetChunkValues(3, targetGridPos + new Vector2Int(-1, 0));
            SetChunkValues(4, targetGridPos + new Vector2Int(0, 0));
            SetChunkValues(5, targetGridPos + new Vector2Int(1, 0));
            SetChunkValues(6, targetGridPos + new Vector2Int(-1, 1));
            SetChunkValues(7, targetGridPos + new Vector2Int(0, 1));
            SetChunkValues(8, targetGridPos + new Vector2Int(1, 1));

            oldCenterChunkIndex = GetTargetChunkGridPosition();
        }

        private bool CheckChunkReplaceable(Vector2Int pos1, Vector2Int pos2)
        {
            if (Mathf.Abs(pos1.x - pos2.x) > 1)
                return true;
            if (Mathf.Abs(pos1.y - pos2.y) > 1)
                return true;

            return false;
        }

        private void SetChunkValues(int chunkIndex, Vector2Int gridPos)
        {
            chunkPool[chunkIndex].transform.localPosition = GridToWorldPosition(gridPos);
            ChunkInfo ci = GetChunkInfoAtChunkCoord(gridPos);
            chunkPool[chunkIndex].SetChunkValueField(ci.valueField, ci.gridPoints);
            chunkPool[chunkIndex].canBeReplaced = false;
        }

        private ChunkInfo GetChunkInfoAtChunkCoord(Vector2Int gridPos)
        {
            if (_caveChunkValues.TryGetValue(gridPos, out var valueField))
                return valueField;
            else
            {
                ChunkInfo newChunkInfo = CreateValueField(GridToWorldPosition(gridPos));
                _caveChunkValues.Add(gridPos, newChunkInfo);
                return newChunkInfo;
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
        private ChunkInfo CreateValueField(Vector3 gridOrigin)
        {
            int size = ChunkSize + 1;
            GridPoint[,] newField = new GridPoint[size, size];

            int index = 0;
            Dictionary<Vector3, int> gridPointDic = new Dictionary<Vector3, int>();

            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                Vector3 gridPointPos = new Vector3(x * CellSize, 0, y * CellSize);
                //float value = Mathf.PerlinNoise(gridPointPos.x / ChunkSize * noiseScale, gridPointPos.z/ ChunkSize * noiseScale);
                float value = 1f;
                //if(x > size/2) value = 0f;

                GridPoint p = new GridPoint(gridPointPos, value);
                p.wallType = GetWallType(gridPointPos);
                newField[x, y] = p;

                gridPointDic.Add(p.pos, index++);
            }

            return new ChunkInfo(newField, gridPointDic);
        }

        private Vector2Int GetTargetChunkGridPosition()
        {
            return WorldToChunkCoord(target.transform.position);
        }

        private Vector2Int WorldToChunkCoord(Vector3 worldPos)
        {
            return new Vector2Int(
                Mathf.FloorToInt(worldPos.x / ChunkSize),
                Mathf.FloorToInt(worldPos.z / ChunkSize));
        }

        private Vector3 GridToWorldPosition(Vector2Int gridPos)
        {
            return new Vector3(gridPos.x * ChunkSize, 0, gridPos.y * ChunkSize);
        }

        private CaveChunk GetChunkThatHoldsValues(GridPoint[,] values)
        {
            for (int i = 0; i < chunkPool.Length; i++)
                if (chunkPool[i].ChunkValueField == values)
                    return chunkPool[i];
            
            return null;
        }

        /// <summary>
        /// Removes part of the  wall at the given position.<br/>
        /// point is the world position of mining, radius the radius and strength is between 0 and 1 the amount subtracted from the wall.
        /// </summary>
        public MiningResult MineWall(Vector3 point, float radius, float strength)
        {
            // TODO: make this cross chunks

            Vector2Int chunkGridPos = WorldToChunkCoord(point);

            GridPoint[,] valueField = GetChunkInfoAtChunkCoord(chunkGridPos).valueField;
            
            // TODO: make this cross chunks
            MiningResult mr = new MiningResult();

            // TODO: maybe optimize 

            for (int y = 0; y < ChunkSize + 1; y++)
                for (int x = 0; x < ChunkSize + 1; x++)
                {
                    Vector3 globalGridPos = valueField[x, y].pos + new Vector3(ChunkSize * chunkGridPos.x, 0, ChunkSize * chunkGridPos.y);
                    float distance = (globalGridPos - point).magnitude;
                    if (distance < radius)
                    {
                        float removeAmount = Mathf.Min(valueField[x, y].value, strength * miningFalloff.Evaluate(distance / radius));
                        valueField[x, y].value -= removeAmount;
                        if (valueField[x, y].wallType == GridPoint.WallType.Stone)
                            mr.stoneAmount += removeAmount;
                        else if (valueField[x, y].wallType == GridPoint.WallType.Coal)
                            mr.coalAmount += removeAmount;
                        else if (valueField[x, y].wallType == GridPoint.WallType.Gold)
                            mr.goldAmount += removeAmount;
                    }
                }


            GetChunkThatHoldsValues(valueField).UpdateMesh();

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

        public void Add(MiningResult mr)
        {
            stoneAmount = mr.stoneAmount;
            coalAmount = mr.coalAmount;
            goldAmount = mr.goldAmount;
        }
    }
}

public struct MeshInfo {
    public int[] indeces;

    public Vector3[] vertices;

    public List<Color> oreUVs;

    public MeshInfo(int[] _indeces, Vector3[] _vertices, List<Color> _oreUVs) 
    {
        indeces = _indeces;
        vertices = _vertices;
        oreUVs = _oreUVs;
    }
}
/// <summary> contains information of a full grid cell </summary>
public struct GridSquare
{
    public GridPoint bottomLeft;
    public GridPoint topLeft;
    public GridPoint topRight;
    public GridPoint bottomRight;

    public GridSquare(GridPoint _bottomLeft,GridPoint _topLeft,GridPoint _topRight,GridPoint _bottomRight) {
        bottomLeft = _bottomLeft;
        topLeft = _topLeft;
        topRight = _topRight;
        bottomRight = _bottomRight;
    }
}