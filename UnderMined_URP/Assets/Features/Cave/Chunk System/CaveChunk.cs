using System;
using UnityEngine;

namespace Features.Cave.Chunk_System
{
    public class CaveChunk : MonoBehaviour
    {
        private GridPoint[,] chunkValueField;

        private float cellSize = 1f;

        private float DEBUG_boxSize = 1f;

        private void Start()
        {
            chunkValueField = new GridPoint[64, 64];
        }

        public void SetChunkValueField(GridPoint[,] valueField)
        {
            chunkValueField = valueField;
        }

        private void OnDrawGizmos()
        {
            if (chunkValueField != null)
            {
                for (int x = 0; x < chunkValueField.GetLength(0); x++)
                {
                    for (int y = 0; y < chunkValueField.GetLength(1); y++)
                    {
                        if (chunkValueField[x, y].wallType == GridPoint.WallType.Gold)
                            Gizmos.color = new Color(1, 1, 0);
                        else if (chunkValueField[x, y].wallType == GridPoint.WallType.Coal)
                            Gizmos.color = new Color(0, 0, 0);
                        else
                        {
                            float g = 0.4f;
                            Gizmos.color = new Color(g, g, g);
                        }

                        if (chunkValueField[x, y].value > 0.5f)
                            Gizmos.DrawCube(chunkValueField[x, y].pos, DEBUG_boxSize * cellSize * Vector3.one);
                    }
                }
            }
        }
    }
}