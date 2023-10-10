using System;
using System.Collections.Generic;
using System.Linq;
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

        public const byte IsoValue = 128;

        public const float WallHeight = 3f;

        public AnimationCurve miningFalloff = AnimationCurve.EaseInOut(0, 1, 0, 0);

        [Header("Chunk Settings:")]
        
        // the alpha is the metallic value
        public Color stoneColor = new Color(0.25f, 0.25f, 0.25f, 0f);
        public Color coalColor = new Color(0, 0f, 0f, 0f);
        public Color goldColor = new Color(0.83f, 0.65f, 0, 1f);
        public Color boosterColor = new Color(0.83f, 0.65f, 0, 1f);

        private class ChunkInfo
        {
            public readonly GridPoint[,] valueField;

            public ChunkInfo(GridPoint[,] valueField)
            {
                this.valueField = valueField;
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

        // contains all modifiedChunks of this frame
        private List<Vector2Int> modifiedChunks = new List<Vector2Int>();

        private Dictionary<Vector2Int, int> chunkCoordToPoolIndex = new Dictionary<Vector2Int, int>();

        public void Awake()
        {
            instance = this;

            randomOffsetPerRun = new Vector3(Random.Range(-200, 200), Random.Range(-1000, 1000), Random.Range(-1000, 1000));
        }

        private void Start()
        {
            oldCenterChunkIndex = GetTargetChunkGridPosition();
            InitializeChunks(oldCenterChunkIndex);
        }

        private void Update()
        {
            Vector2Int targetGridPos = GetTargetChunkGridPosition();

            if (oldCenterChunkIndex != targetGridPos) 
            {
                UpdateChunks(targetGridPos);
            }
        }

        private void InitializeChunks(Vector2Int targetGridPos)
        {
            // hard settings chunks for initialization
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

        private void UpdateChunks(Vector2Int targetGridPos)
        {
            List<Vector2Int> requiredChunks = new List<Vector2Int>();

            List<Vector2Int> currentChunkCoords = chunkCoordToPoolIndex.Keys.ToList();

            requiredChunks.Add(targetGridPos);

            // required neighbor chunks
            requiredChunks.Add(targetGridPos + new Vector2Int(-1,    -1));
            requiredChunks.Add(targetGridPos + new Vector2Int( 0,    -1));
            requiredChunks.Add(targetGridPos + new Vector2Int( 1,    -1));
            requiredChunks.Add(targetGridPos + new Vector2Int(-1,     0));
            requiredChunks.Add(targetGridPos + new Vector2Int( 1,     0));
            requiredChunks.Add(targetGridPos + new Vector2Int(-1,     1));
            requiredChunks.Add(targetGridPos + new Vector2Int( 0,     1));
            requiredChunks.Add(targetGridPos + new Vector2Int( 1,     1));

            foreach(Vector2Int requiredChunk in requiredChunks)
            {
                // this required chunk must be added
                if(!currentChunkCoords.Contains(requiredChunk))
                {
                    // must remove replaced chunk, otherwise, we try to replace it again
                    int replacedChunkIndex = -1;

                    // find replacable spot
                    for(int i = 0; i < currentChunkCoords.Count; i++) 
                    {
                        Vector2Int currentChunk = currentChunkCoords[i];

                        Vector2Int targetToCurrent = currentChunk - targetGridPos;
                        // if current saved chunk too far away, we can replace it
                        if (Mathf.Abs(targetToCurrent.x) > 1 || Mathf.Abs(targetToCurrent.y) > 1)
                        {
                            int replacableIndex = chunkCoordToPoolIndex[currentChunk];

                            chunkCoordToPoolIndex.Remove(currentChunk);

                            replacedChunkIndex = i;
                            
                            // replace chunk
                            SetChunkValues(replacableIndex, requiredChunk);
                            break;
                        }
                    }

                    currentChunkCoords.RemoveAt(replacedChunkIndex);
                }
            }

            oldCenterChunkIndex = targetGridPos;
        }

        private void LateUpdate()
        {
            UpdateMeshesOfModifiedChunks();
        }

        private void SetChunkValues(int chunkIndex, Vector2Int gridPos)
        {
            chunkPool[chunkIndex].transform.localPosition = GridToWorldPosition(gridPos);
            ChunkInfo ci = GetChunkInfoAtChunkCoord(gridPos);
            chunkCoordToPoolIndex.Add(gridPos, chunkIndex);
            chunkPool[chunkIndex].SetChunkValueField(ci.valueField);
            chunkPool[chunkIndex].chunkGridPos = gridPos;
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

        private WallType GetWallType(Vector3 position)
        {
            bool isCoal = OreGeneratorFunction(position + new Vector3(0, 0, 100));
            if (isCoal)
                return WallType.Coal;

            bool isGold = OreGeneratorFunction(position + new Vector3(100, 0, 0));
            if (isGold)
                return WallType.Gold;
            
            bool isBooster = OreGeneratorFunction(position + new Vector3(-100, 0, 0));
            if (isBooster)
                return WallType.Booster;

            return WallType.Stone;
        }

        private bool OreGeneratorFunction(Vector3 position)
        {
            float oreValue = 1f;

            position += randomOffsetPerRun;

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

            Dictionary<Vector3, int> gridPointDic = new Dictionary<Vector3, int>();

            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                Vector3 gridPointPos = new Vector3(x * CellSize, 0, y * CellSize);
                //float value = Mathf.PerlinNoise(gridPointPos.x / ChunkSize * noiseScale, gridPointPos.z/ ChunkSize * noiseScale);
                Byte value = 255;
                //if(x > size/2) value = 0f;

                GridPoint p = new GridPoint(value, GetWallType(gridOrigin + gridPointPos));
                newField[x, y] = p;
            }

            return new ChunkInfo(newField);
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

        private CaveChunk GetChunkThatHoldsValues(Vector2Int chunkGridPos)
        {
            for (int i = 0; i < chunkPool.Length; i++)
                if (chunkPool[i].chunkGridPos == chunkGridPos)
                    return chunkPool[i];

            return null;
        }

        /// <summary>
        /// transform the global position to a local pos in a chunk
        /// </summary>
        /// <param name="globalPos"></param>
        /// <returns>Vector2 containing float values from 0 to Chunksize</returns>
        private Vector2 GlobalPosToLocalGridPos(Vector3 globalPos)
        {
            Vector2 localMinePoint = new Vector2();

            if (globalPos.x < 0)
            {
                localMinePoint.x = ChunkSize - Mathf.Abs(globalPos.x) % ChunkSize;
            }
            else
            {
                localMinePoint.x = globalPos.x % ChunkSize;
            }

            if (globalPos.z < 0)
            {
                localMinePoint.y = ChunkSize - Mathf.Abs(globalPos.z) % ChunkSize;
            }
            else
            {
                localMinePoint.y = globalPos.z % ChunkSize;
            }
            return localMinePoint;
        }

        /// <summary>
        /// Removes part of the  wall at the given position.<br/>
        /// point is the world position of mining, radius the radius and strength is between 0 and 1 the amount subtracted from the wall.
        /// </summary>
        public OreCollection MineWall(Vector3 point, float radius, float strength)
        {
            Vector2Int chunkGridPos = WorldToChunkCoord(point);

            // transform the global position to a local 0-Chunksize pos
            Vector2 localMinePoint = GlobalPosToLocalGridPos(point);

            OreCollection ore = MineChunk(point, radius, strength, chunkGridPos, localMinePoint, chunkGridPos);

            OreCollection oreFromOtherChunk;

            void MineNeighbor(Vector2Int dir) {
                oreFromOtherChunk = MineChunk(point, radius, strength, chunkGridPos + dir, localMinePoint, chunkGridPos);
                ore.AddOreCollection(oreFromOtherChunk);
            }

            bool goUp = localMinePoint.y + radius > ChunkSize - 1;
            bool goDown = localMinePoint.y - radius < 0;
            bool goRight = localMinePoint.x + radius > ChunkSize - 1;
            bool goLeft = localMinePoint.x - radius < 0;

            if (goRight && goUp) MineNeighbor(Vector2Int.right + Vector2Int.up);
            if (goRight && goDown) MineNeighbor(Vector2Int.right + Vector2Int.down);
            if (goLeft && goUp) MineNeighbor(Vector2Int.left + Vector2Int.up);
            if (goLeft && goDown) MineNeighbor(Vector2Int.left + Vector2Int.down);
            if (goUp) MineNeighbor(Vector2Int.up);
            if (goDown) MineNeighbor(Vector2Int.down);
            if (goRight) MineNeighbor(Vector2Int.right);
            if (goLeft) MineNeighbor(Vector2Int.left);

            return ore;
        }

        private OreCollection MineChunk(Vector3 point, float radius, float strength, Vector2Int chunkGridPos, Vector2 localMinePoint, Vector2Int originChunk) {
            GridPoint[,] valueField = GetChunkInfoAtChunkCoord(chunkGridPos).valueField;
            
            OreCollection ore = new OreCollection();

            int gridRadius = Mathf.FloorToInt(radius / CellSize);

            // floor both coords and later add +1 to the range for symmetry
            Vector2Int gridCoords = new Vector2Int(Mathf.FloorToInt(localMinePoint.x),Mathf.FloorToInt(localMinePoint.y));

            // spans across all grid points that could possibly be affected by mining

            int yStart;
            int yEnd;
            int xStart;
            int xEnd;

            // transform bound coords from origin chunk to neighbor chunk
            int HandleEdgeCases(int originChunkCoord)
            {
                int result;
                // the minus 2 keeps consistency with the range
                if (originChunkCoord <= ChunkSize && originChunkCoord >= ChunkSize - 2*gridRadius - 2)
                {
                    // upround case
                    result = 0;
                } else if (originChunkCoord >= 0 && originChunkCoord <= 2*gridRadius)
                {
                    // downround case
                    result = ChunkSize + 1;
                } else if (originChunkCoord >= ChunkSize)
                {
                    // overshoot case
                    result = originChunkCoord % ChunkSize;
                } else if(originChunkCoord <= 0)
                {
                    // undershoot case
                    result = ChunkSize + originChunkCoord;
                } else
                {
                    // error case
                    result = -1;
                }

                return result;
            }

            // transform minepoint from originchunk to neighbor chunk
            float HandleMinePointCases(float mineCoord)
            {
                if (mineCoord < radius)
                {
                    return ChunkSize + mineCoord;
                }
                else
                {
                    return mineCoord - ChunkSize;
                }
            }

            // the additional length is done to keep the mining symmetrical
            int startCoordY = gridCoords.y - gridRadius;
            int endCoordY = gridCoords.y + gridRadius + 1;
            int startCoordX = gridCoords.x - gridRadius;
            int endCoordX = gridCoords.x + gridRadius + 1;

            // if we are in a neighborchunk, we must convert all coords fitting for that chunk
            if(chunkGridPos.y == originChunk.y)
            {
                yStart = Mathf.Max(0, startCoordY);
                yEnd = Mathf.Min(ChunkSize + 1, endCoordY);
            } else
            {
                yStart = HandleEdgeCases(startCoordY);
                yEnd = HandleEdgeCases(endCoordY);

                localMinePoint.y = HandleMinePointCases(localMinePoint.y);
            }
            if(chunkGridPos.x == originChunk.x)
            {
                xStart = Mathf.Max(0, startCoordX);
                xEnd = Mathf.Min(ChunkSize + 1, endCoordX);
            } else
            {
                xStart = HandleEdgeCases(startCoordX);
                xEnd = HandleEdgeCases(endCoordX);

                localMinePoint.x = HandleMinePointCases(localMinePoint.x);
            }

            // go through all nearby points to mining point
            for (int y = yStart; y < yEnd; y++)
            {
                for (int x = xStart; x < xEnd; x++)
                {
                    float distance = (localMinePoint - new Vector2(x,y)).magnitude;
                    if (distance < radius)
                    {
                        byte removeAmount = Math.Min(valueField[x, y].value, (byte)(strength * miningFalloff.Evaluate(distance / radius) * 255f));
                        valueField[x, y].value -= removeAmount;

                        ore.AddOre(valueField[x, y].wallType, removeAmount);
                    }
                }
            }

            // add the chunk only if it hasn't been added yet
            if(!modifiedChunks.Contains(chunkGridPos)) modifiedChunks.Add(chunkGridPos);

            // GetChunkThatHoldsValues(chunkGridPos).UpdateMesh();

            return ore;
        }

        private void UpdateMeshesOfModifiedChunks()
        {
            foreach (var chunkGridPos in modifiedChunks)
            {
                GetChunkThatHoldsValues(chunkGridPos).UpdateMesh();
            }
            // clear the list for next frame
            modifiedChunks.Clear();
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