using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    

    private GridPoint[,] map;

    private void Start()
    {
        InitializeMap(transform.position);
    }

    /// <summary>
    /// fills <see cref="map"/> with <see cref="GridPoint"/>s
    /// </summary>
    /// <param name="gridOrigin"> is world space pos at the bottom left of the grid</param>
    private void InitializeMap(Vector3 gridOrigin)
    {
        map = new GridPoint[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 gridPointPos = gridOrigin + x * cellSize * Vector3.right + y * cellSize * Vector3.forward;
                map[x, y] = new GridPoint(gridPointPos);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Gizmos.color = (map[x,y].value == 1)?Color.black:Color.white;
                    Gizmos.DrawCube(map[x, y].pos, cellSize * 0.5f * Vector3.one);
                }
            }
        }
    }
}
