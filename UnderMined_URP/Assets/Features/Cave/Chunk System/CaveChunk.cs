using System;
using System.Collections.Generic;
using UnityEngine;

namespace Features.Cave.Chunk_System
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class CaveChunk : MonoBehaviour
    {
        private GridPoint[,] chunkValueField;
        public GridPoint[,] ChunkValueField => chunkValueField;
        private Dictionary<Vector3, int> gridPointDic;

        private MeshGenerator _meshGenerator;
        private MeshFilter _meshFilter;

        public bool canBeReplaced = true;

        private float DEBUG_boxSize = 1f;

        [SerializeField]
        private GameObject wallChild;
        private MeshFilter _wallFilter;

        private MeshCollider _wallCollider;

        private void Awake()
        {
            _meshGenerator = new MeshGenerator();
            _meshFilter = GetComponent<MeshFilter>();

            _wallFilter = wallChild.GetComponent<MeshFilter>();
            _wallCollider = wallChild.GetComponent<MeshCollider>();
        }

        private void Start()
        {
            chunkValueField = new GridPoint[ChunkManager.ChunkSize, ChunkManager.ChunkSize];
        }

        public void SetChunkValueField(GridPoint[,] valueField, Dictionary<Vector3, int> gridPoints)
        {
            chunkValueField = valueField;
            gridPointDic = gridPoints;

            // generate mesh
            UpdateMesh();
        }

        public void UpdateMesh()
        {
            // generate mesh for top and walls
            
            MeshInfo[] meshInfos =_meshGenerator.GenerateMeshFromMap(chunkValueField, gridPointDic, ChunkManager.IsoValue, ChunkManager.WallHeight);
            
            // assign top
            MeshInfo topInfo = meshInfos[0];
            Mesh top = new Mesh();
            top.name = "top";

            top.SetVertices(topInfo.vertices);
            top.SetIndices(topInfo.indeces, MeshTopology.Triangles, 0);
            top.SetUVs(1,topInfo.oreUVs);
            top.RecalculateNormals();

            _meshFilter.mesh = top;

            // assign walls
            MeshInfo wallInfo = meshInfos[1];
            Mesh wall = new Mesh();
            wall.name = "wall";

            wall.SetVertices(wallInfo.vertices);
            wall.SetIndices(wallInfo.indeces, MeshTopology.Triangles, 0);
            wall.RecalculateNormals();


            _wallFilter.mesh = wall;
            _wallCollider.sharedMesh = wall;
        }

        public bool DEBUG_drawValueField = true;
        private void OnDrawGizmos()
        {
            if (DEBUG_drawValueField == false) 
                return;
            
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

                        if (chunkValueField[x, y].value > ChunkManager.IsoValue)
                            Gizmos.DrawCube(chunkValueField[x, y].pos, chunkValueField[x, y].value * DEBUG_boxSize * ChunkManager.CellSize * Vector3.one);
                    }
                }
            }
            
            /* 
            // Draw Squares
            if(squares != null)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    for (int y = 0; y < height - 1; y++)
                    {
                        Gizmos.color = Color.red;

                        GridSquare s = squares[x,y];

                        Gizmos.DrawLine(s.bottomLeft.pos,s.topLeft.pos);
                        Gizmos.DrawLine(s.topLeft.pos,s.topRight.pos);
                        Gizmos.DrawLine(s.topRight.pos,s.bottomRight.pos);
                        Gizmos.DrawLine(s.bottomRight.pos,s.bottomLeft.pos);
                    }
                }
            }
            */

            /* 
            // Draw Triangles
            if(Application.isPlaying) {
                Gizmos.color = Color.red;
                int triangleCount = meshInfo.indeces.Length / 3;
                Vector3[] verts = meshInfo.vertices;
    
                int[] indeces = meshInfo.indeces;
                for(int i = 0; i < triangleCount - 1; i++) {
                    int startIndex = i * 3;
                    
                    Vector3 a = verts[indeces[startIndex]];
                    Vector3 b = verts[indeces[startIndex+1]];
                    Vector3 c = verts[indeces[startIndex+2]];
    
                    Gizmos.DrawLine(a,b);
                    Gizmos.DrawLine(b,c);
                    Gizmos.DrawLine(c,a);
                }
            }
            */

        }
    }
}