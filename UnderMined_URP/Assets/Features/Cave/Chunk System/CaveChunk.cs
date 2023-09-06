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
                        float value = chunkValueField[x, y].value;

                        if (chunkValueField[x, y].value == 0.5f)
                            Gizmos.color = new Color(value, 0, 0);
                        else
                            Gizmos.color = new Color(0, 0, value);
                        
                        Gizmos.DrawCube(chunkValueField[x, y].pos, value * cellSize * DEBUG_boxSize * Vector3.one);
                    }
                }
            }
        }
    }
}
