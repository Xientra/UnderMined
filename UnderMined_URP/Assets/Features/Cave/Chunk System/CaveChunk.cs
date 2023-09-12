using System;
using System.Collections.Generic;
using UnityEngine;

namespace Features.Cave.Chunk_System
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class CaveChunk : MonoBehaviour
    {
        /// <summary>contains local position at grid point and the value there </summary>
        private GridPoint[,] chunkValueField;
        public GridPoint[,] ChunkValueField => chunkValueField;

        private MeshGenerator _meshGenerator;

        private MeshFilter _meshFilter;

        public bool canBeReplaced = true;

        private float DEBUG_boxSize = 1f;

       
        [SerializeField]
        [Tooltip("Object for chunk wall; must have a MeshFilter + MeshCollider components")]
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

        public void SetChunkValueField(GridPoint[,] valueField)
        {
            chunkValueField = valueField;

            // generate mesh
            UpdateMesh();
        }

        public void UpdateMesh()
        {
            // generate mesh for top and walls
            
            Mesh[] meshInfos =_meshGenerator.GenerateMeshFromMap(chunkValueField, ChunkManager.IsoValue, ChunkManager.WallHeight);
            
            // assign top
            Mesh top = meshInfos[0];
            top.name = "top";

            Vector3[] topVertices = top.vertices;

            // this part is for shading of mesh
            //List<Vector2> ores = new List<Vector2>();
            List<Color> oreColor = new List<Color>();

            Vector2 WallToUV(GridPoint p) {return new Vector2((int)p.wallType, p.value);}

            for(int i = 0; i < topVertices.Length; i++) {
                Vector3 v = topVertices[i];
                int x = Mathf.RoundToInt(v.x);
                int y = Mathf.RoundToInt(v.z);

                if(x < 0 || x > ChunkManager.ChunkSize || y < 0 || y > ChunkManager.ChunkSize) {
                    //ores.Add((int)GridPoint.WallType.Stone * Vector2.right);
                    oreColor.Add(Color.magenta);
                } else {
                    GridPoint p = ChunkValueField[x,y];
                    //ores.Add(WallToUV(p));

                  
                    switch(p.wallType){
                        case WallType.Stone: oreColor.Add(ChunkManager.instance.stoneColor);
                            break;
                        case WallType.Coal: oreColor.Add(ChunkManager.instance.coalColor);
                            break;
                        case WallType.Gold: oreColor.Add(ChunkManager.instance.goldColor);
                            break;
                        case WallType.Booster: oreColor.Add(ChunkManager.instance.boosterColor);
                            break;
                    }
                }
            }

            //top.SetUVs(0,ores);
            top.SetColors(oreColor);
            top.RecalculateNormals();

            _meshFilter.mesh = top;

            // assign walls
            Mesh wall = meshInfos[1];
            wall.name = "wall";

            _wallFilter.mesh = wall;
            _wallCollider.sharedMesh = wall;
        }

        public bool DEBUG_drawValueField = true;
        private void OnDrawGizmosSelected()
        {
            if (DEBUG_drawValueField == false) 
                return;
            
            if (chunkValueField != null)
            {
                for (int x = 0; x < chunkValueField.GetLength(0); x++)
                {
                    for (int y = 0; y < chunkValueField.GetLength(1); y++)
                    {
                        if (chunkValueField[x, y].wallType == WallType.Gold)
                            Gizmos.color = new Color(1, 1, 0);
                        else if (chunkValueField[x, y].wallType == WallType.Coal)
                            Gizmos.color = new Color(0, 0, 0);
                        else
                        {
                            float g = 0.4f;
                            Gizmos.color = new Color(g, g, g);
                        }

                        if (chunkValueField[x, y].value > ChunkManager.IsoValue)
                            Gizmos.DrawCube(transform.position + chunkValueField[x, y].pos, chunkValueField[x, y].value * DEBUG_boxSize * ChunkManager.CellSize * Vector3.one);
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