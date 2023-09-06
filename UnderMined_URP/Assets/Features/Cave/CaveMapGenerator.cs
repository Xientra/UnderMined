using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public struct MeshInfo {
    public int[] indeces;

    public Vector3[] vertices;

    public MeshInfo(int[] _indeces, Vector3[] _vertices) 
    {
        indeces = _indeces;
        vertices = _vertices;
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

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CaveMapGenerator : MonoBehaviour
{
    [SerializeField]
    [Tooltip("determines number of columns in grid")]
    private int width = 64;

    [SerializeField]
    [Tooltip("determines number of rows in grid")]
    private int height = 64;

    [SerializeField]
    [Tooltip("determines side length of one Cell")]
    private float cellSize = 1f;

    [SerializeField]
    [Tooltip("determines what's in and out")]
    private float isoValue = .5f;
    
    private MeshGenerator meshGenerator;

    private GridPoint[,] map;

    private MeshInfo meshInfo;

    private MeshFilter meshFilter;

    private void Start()
    {
        meshGenerator = new MeshGenerator();

        meshFilter = GetComponent<MeshFilter>();

        InitializeMap(transform.position);
    }

    /// <summary>
    /// fills <see cref="map"/> with <see cref="GridPoint"/>s
    /// </summary>
    /// <param name="gridOrigin"> is world space pos at the bottom left of the grid</param>
    private void InitializeMap(Vector3 gridOrigin)
    {
        map = new GridPoint[width, height];
        Dictionary<Vector3, int> gridPointDic = new Dictionary<Vector3, int>();

        int index = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 gridPointPos = gridOrigin + x * cellSize * Vector3.right + y * cellSize * Vector3.forward;
                GridPoint p = new GridPoint(gridPointPos);
                map[x, y] = p;

                gridPointDic.Add(p.pos, index++);
            }
        }

        MeshInfo[] meshInfos = meshGenerator.GenerateMeshFromMap(map, gridPointDic, isoValue);

        MeshInfo meshInfo = meshInfos[0];

        Mesh mesh = new Mesh();

        mesh.SetVertices(meshInfo.vertices);
        mesh.SetIndices(meshInfo.indeces, MeshTopology.Triangles, 0);
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

        // this can give me the gridpoint in the map based on its index (which goes from 0 to ((width*height) - 1))
        //GridPoint p = (GridPoint)map[(int)Mathf.Ceil(index/width), index%width];
    }

    private void OnDrawGizmos()
    {
        // Draw GridPoints
        if(map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float v = map[x,y].value;
                    //Gizmos.color = new Color(v,v,v);
                    Gizmos.color = v > isoValue ? Color.black : Color.white;
                    Gizmos.DrawCube(map[x, y].pos, cellSize * 0.1f * Vector3.one);
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
