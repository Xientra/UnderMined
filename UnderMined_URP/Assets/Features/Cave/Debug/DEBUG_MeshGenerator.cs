using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUG_MeshGenerator : MonoBehaviour
{
    public float[,] grid = { { 1, 1 }, { 1, 0 } };

    MeshFilter meshFilter;

    Mesh mesh;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();

        mesh = new Mesh();
    }

    private void Update()
    {
        int x = grid.GetLength(0);
        int y = grid.GetLength(1);

        GridPoint[,] map = new GridPoint[x,y];

        MeshGenerator gen = new MeshGenerator();

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                map[i, j] = new GridPoint(new Vector3(i, j), grid[i, j], 0);
            }
        }

        Mesh[] infos = gen.GenerateMeshFromMap(map, 0.1f, 3);

        mesh = infos[0];

        meshFilter.mesh = mesh;
    }

}
