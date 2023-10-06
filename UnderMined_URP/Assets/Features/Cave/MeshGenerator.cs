using Features.Cave.Chunk_System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator
{
    /// <summary>
    /// Implementation of a Marching Squares Algorithm
    /// </summary>
    /// <param name="map">contains the signed distance field values with local positions</param>
    /// <param name="gridPointDic">contains local grid positions as keys and their vertex index as value</param>
    /// <param name="isoValue">determines what counts as wall/air. Values lower than this count as air</param>
    /// <param name="wallHeight"></param>
    /// <returns>two <see cref="Mesh"/>s in an Array. At [0] is the top of the mesh. At [1] are the walls</returns>
    public Mesh[] GenerateMeshFromMap(GridPoint[,] map, float isoValue, float wallHeight) {
        int width = map.GetLength(0) - 1;
        int height = map.GetLength(1) - 1;
        // create squares in map
        GridSquare[,] squares = GenerateGridSquares(map, width, height);

        // keeps track of used Vertices
        Dictionary<int, Vector3> indexToVertex = new Dictionary<int, Vector3>();
        Dictionary<Vector3, int> vertexToIndex = new Dictionary<Vector3, int>();

        List<int> outlines = new List<int>();

        // growing list of triangle indeces
        List<int> indeces = new List<int>();

        // growing list of vertex positions
        List<Vector3> vertices = new List<Vector3>();

        // march through squares a second time
        for (int y = 0; y < width; y++)
        {
            for (int x = 0; x < height; x++)
            {
                TriangulateSquare(squares[x,y], x, y, isoValue, indeces, vertices, outlines, indexToVertex, vertexToIndex);
            }
        }

        int[] indecesArr = indeces.ToArray();

        Mesh[] result = new Mesh[2];

        Mesh topMesh = new Mesh();
        topMesh.SetVertices(vertices);
        topMesh.SetIndices(indecesArr, MeshTopology.Triangles, 0);
        topMesh.RecalculateNormals();

        result[0] = topMesh;
        result[1] = TriangulateWall(wallHeight, outlines, vertices);

        return result;
    }

    private Mesh TriangulateWall(float wallheight, List<int> outlineIndeces, List<Vector3> vertices) {
        Mesh mesh = new();
        int wallSegmentCount = outlineIndeces.Count / 2;

        int wallIndex = 0;

        List<Vector3> wallVerts = new List<Vector3>();
        List<int> wallIndeces = new List<int>();

        Dictionary<int,int> topIndexToWallIndex = new Dictionary<int,int>();
        Dictionary<Vector3, int> wallPosToIndex = new Dictionary<Vector3, int>();

        int assignTop(int wallSegmentIndex)
        {
            int vertexIndex = outlineIndeces[wallSegmentIndex];

            if(topIndexToWallIndex.TryGetValue(vertexIndex, out int wallVertIndex))
            {
                return wallVertIndex;
            } else
            {
                wallVerts.Add(vertices[vertexIndex]);
                int nextIndex = wallIndex++;
                topIndexToWallIndex[vertexIndex] = nextIndex;
                return nextIndex;
            }
        }

        int assignBottom(int wallSegmentIndex)
        {
            Vector3 originPos = vertices[outlineIndeces[wallSegmentIndex]];
            Vector3 wallPos = originPos + Vector3.down * wallheight;
            if (wallPosToIndex.TryGetValue(wallPos, out int wallVertIndex)) 
            {
                return wallVertIndex; 
            } else
            {
                wallVerts.Add(wallPos);
                int nextIndex = wallIndex++;
                wallPosToIndex[wallPos] = nextIndex;
                return nextIndex;
            }
        }
        
        for(int i = 0; i < wallSegmentCount; i++) {
            int wallSegmentIndex = i * 2;

            int aI = assignTop(wallSegmentIndex);
            int bI = assignBottom(wallSegmentIndex);
            int cI = assignBottom(wallSegmentIndex + 1);
            int dI = assignTop(wallSegmentIndex + 1);

            wallIndeces.Add(aI);
            wallIndeces.Add(cI);
            wallIndeces.Add(bI);
            
            wallIndeces.Add(aI);
            wallIndeces.Add(dI);
            wallIndeces.Add(cI);
        }

        mesh.SetVertices(wallVerts);
        mesh.SetIndices(wallIndeces, MeshTopology.Triangles, 0);
        mesh.RecalculateNormals();

        if (wallVerts.Count < 3 && wallVerts.Count > 0)
        {
            Debug.LogError("whyyyyyy");
            Debug.Log(wallVerts.Count);
            for(int i = 0;i < wallVerts.Count;i++)
            {
                Debug.Log("Vertex: " + wallVerts[i] + " at index " + i);
            }
        }

        return mesh;
    }

    private GridSquare[,] GenerateGridSquares(GridPoint[,] map, int width, int height) {
        GridSquare[,] squares = new GridSquare[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // create a square of gridPoints
                GridSquare s = new GridSquare(map[x,y],map[x,y+1],map[x+1,y+1],map[x+1,y]);

                squares[x,y] = s;
            }
        }
        return squares;
    }

    private void TriangulateSquare(GridSquare square, int x, int y, float isoValue, List<int> indeces, List<Vector3> vertices, List<int> outlines
        , Dictionary<int, Vector3> indexToVertex, Dictionary<Vector3, int> vertexToIndex) {
        /// <summary>
        /// to find position at which isoContour to the isoValue is
        /// </summary>
        /// <param name="lower"> is world space pos at the bottom left of the grid</param>
        /// <param name="higher"> is world space pos at the bottom left of the grid</param>
        /// <param name="isoValue"> is world space pos at the bottom left of the grid</param>
        Vector3 CrossPos(byte lowerValue, Vector3 lowerPos, byte higherValue, Vector3 higherPos, float isoValue) {
            Vector3 result = Vector3.zero;
            if(!lowerValue.Equals(higherValue)) {
                float t = (isoValue - lowerValue) / (higherValue - lowerValue);
                result = (1-t) * lowerPos + t * higherPos;
            }
            return result;
        }

        // predefinitions
        // index just counts upwards depending on length of gridpointDic
        int currentIndex = vertices.Count;

        GridPoint bL = square.bottomLeft;
        GridPoint tL = square.topLeft;
        GridPoint tR = square.topRight;
        GridPoint bR = square.bottomRight;

        float s = ChunkManager.CellSize;

        // corner points
        Vector3 bLpos = new(x, 0, y);
        Vector3 tLpos = new(x, 0, y + s);
        Vector3 tRpos = new(x + s , 0,y + s);
        Vector3 bRpos = new(x + s, 0, y);

        // points on square sides
        Vector3 l;
        Vector3 t;
        Vector3 r;
        Vector3 b;

        // corner indices
        int bLI;
        int tLI;
        int tRI;
        int bRI;

        // side indices
        int lI;
        int tI;
        int rI;
        int bI;

        /// <summary> if the vertex position has been added already, take the index of the existing vertex otherwise choose next bigger index and add it to dictionary </summary>
        int assignIndex(Vector3 pos)
        {
            int index;
            if (vertexToIndex.TryGetValue(pos, out int value))
            {
                index = value;
            }
            else
            {
                index = currentIndex++;
                vertices.Add(pos);
                vertexToIndex.Add(pos, index);
                indexToVertex.Add(index, pos);
            }
            return index;
        }

        int caseNumber = 0;

        // adding the bit-value number based on pos in square
        if(bL.value > isoValue) {
            caseNumber += 8;
        }
        if(tL.value > isoValue) {
            caseNumber += 4;
        }
        if(tR.value > isoValue) {
            caseNumber += 2;
        }
        if(bR.value > isoValue) {
            caseNumber += 1;
        }

        switch(caseNumber) {
            case 0:
            // all empty case
            break;
            case 1:
                b = CrossPos(bL.value,bLpos,bR.value,bRpos,isoValue);
                r = CrossPos(tR.value,tRpos,bR.value,bRpos,isoValue);

                bI = assignIndex(b);
                rI = assignIndex(r);
                bRI = assignIndex(bRpos);

                indeces.Add(bI);
                indeces.Add(rI);
                indeces.Add(bRI);

                outlines.Add(rI);
                outlines.Add(bI);
            break;
            case 2:
                r = CrossPos(bR.value,bRpos,tR.value,tRpos,isoValue);
                t = CrossPos(tL.value,tLpos,tR.value,tRpos,isoValue);

                rI = assignIndex(r);
                tI = assignIndex(t);
                tRI = assignIndex(tRpos);

                indeces.Add(rI);
                indeces.Add(tI);
                indeces.Add(tRI);

                outlines.Add(tI);
                outlines.Add(rI);
            break;
            case 3:
                b = CrossPos(bL.value,bLpos,bR.value,bRpos,isoValue);
                t = CrossPos(tL.value,tLpos,tR.value,tRpos,isoValue);

                bI = assignIndex(b);
                tI = assignIndex(t);
                tRI = assignIndex(tRpos);
                bRI = assignIndex(bRpos);

                indeces.Add(bI);
                indeces.Add(tRI);
                indeces.Add(bRI);

                indeces.Add(bI);
                indeces.Add(tI);
                indeces.Add(tRI);

                outlines.Add(tI);
                outlines.Add(bI);
            break;
            case 4:
                t = CrossPos(tR.value,tRpos,tL.value,tLpos,isoValue);
                l = CrossPos(bL.value,bLpos,tL.value,tLpos,isoValue);

                tI = assignIndex(t);
                lI = assignIndex(l);
                tLI = assignIndex(tLpos);

                indeces.Add(lI);
                indeces.Add(tLI);
                indeces.Add(tI);

                outlines.Add(lI);
                outlines.Add(tI);
            break;
            case 5:
                l = CrossPos(bL.value,bLpos,tL.value,tLpos,isoValue);
                b = CrossPos(bL.value,bLpos,bR.value,bRpos,isoValue);

                r = CrossPos(tR.value,tRpos,bR.value,bRpos,isoValue);
                t = CrossPos(tR.value,tRpos,tL.value,tLpos,isoValue);

                lI = assignIndex(l);
                bI = assignIndex(b);

                rI = assignIndex(r);
                tI = assignIndex(t);

                tLI = assignIndex(tLpos);
                bRI = assignIndex(bRpos);

                indeces.Add(tLI);
                indeces.Add(bI);
                indeces.Add(lI);

                indeces.Add(tLI);
                indeces.Add(tI);
                indeces.Add(bI);

                indeces.Add(tI);
                indeces.Add(bRI);
                indeces.Add(bI);

                indeces.Add(tI);
                indeces.Add(rI);
                indeces.Add(bRI);
            
                outlines.Add(bI);
                outlines.Add(lI);
            
                outlines.Add(tI);
                outlines.Add(rI);
                break;
            case 6:
                l = CrossPos(bL.value,bLpos,tL.value,tLpos,isoValue);
                r = CrossPos(bR.value,bRpos,tR.value,tRpos,isoValue);

                lI = assignIndex(l);
                rI = assignIndex(r);

                tLI = assignIndex(tLpos);
                tRI = assignIndex(tRpos);

                indeces.Add(lI);
                indeces.Add(tLI);
                indeces.Add(tRI);

                indeces.Add(lI);
                indeces.Add(tRI);
                indeces.Add(rI);

                outlines.Add(lI);
                outlines.Add(rI);
                break;
            case 7:
                l = CrossPos(bL.value,bLpos,tL.value,tLpos,isoValue);
                b = CrossPos(bL.value,bLpos,bR.value,bRpos,isoValue);

                lI = assignIndex(l);
                bI = assignIndex(b);

                tLI = assignIndex(tLpos);
                tRI = assignIndex(tRpos);
                bRI = assignIndex(bRpos);

                indeces.Add(lI);
                indeces.Add(tLI);
                indeces.Add(tRI);

                indeces.Add(lI);
                indeces.Add(tRI);
                indeces.Add(bI);

                indeces.Add(bI);
                indeces.Add(tRI);
                indeces.Add(bRI);

                outlines.Add(lI);
                outlines.Add(bI);
            break;
            case 8:
                l = CrossPos(tL.value,tLpos,bL.value,bLpos,isoValue);
                b = CrossPos(bR.value,bRpos,bL.value,bLpos,isoValue);

                lI = assignIndex(l);
                bI = assignIndex(b);
                bLI = assignIndex(bLpos);

                indeces.Add(lI);
                indeces.Add(bI);
                indeces.Add(bLI);

                outlines.Add(bI);
                outlines.Add(lI);
            break;
            case 9:
                l = CrossPos(tL.value,tLpos,bL.value,bLpos,isoValue);
                r = CrossPos(tR.value,tRpos,bR.value,bRpos,isoValue);

                lI = assignIndex(l);
                rI = assignIndex(r);
                bLI = assignIndex(bLpos);
                bRI = assignIndex(bRpos);

                indeces.Add(lI);
                indeces.Add(rI);
                indeces.Add(bLI);

                indeces.Add(bLI);
                indeces.Add(rI);
                indeces.Add(bRI);

                outlines.Add(rI);
                outlines.Add(lI);
            break;
            case 10:
                t = CrossPos(tL.value,tLpos,tR.value,tRpos,isoValue);
                l = CrossPos(tL.value,tLpos,bL.value,bLpos,isoValue);

                b = CrossPos(bR.value,bRpos,bL.value,bLpos,isoValue);
                r = CrossPos(bR.value,bRpos,tR.value,tRpos,isoValue);

                tI = assignIndex(t);
                lI = assignIndex(l);
                bI = assignIndex(b);
                rI = assignIndex(r);
                bLI = assignIndex(bLpos);
                tRI = assignIndex(tRpos);

                indeces.Add(lI);
                indeces.Add(bI);
                indeces.Add(bLI);

                indeces.Add(lI);
                indeces.Add(tI);
                indeces.Add(bI);

                indeces.Add(tI);
                indeces.Add(rI);
                indeces.Add(bI);

                indeces.Add(tI);
                indeces.Add(tRI);
                indeces.Add(rI);

                outlines.Add(lI);
                outlines.Add(tI);

                outlines.Add(rI);
                outlines.Add(bI);
            break;
            case 11:
                t = CrossPos(tL.value,tLpos,tR.value,tRpos,isoValue);
                l = CrossPos(tL.value,tLpos,bL.value,bLpos,isoValue);

                tI = assignIndex(t);
                lI = assignIndex(l);
                bRI = assignIndex(bRpos);
                bLI = assignIndex(bLpos);
                tRI = assignIndex(tRpos);

                indeces.Add(lI);
                indeces.Add(bRI);
                indeces.Add(bLI);

                indeces.Add(lI);
                indeces.Add(tI);
                indeces.Add(bRI);

                indeces.Add(tI);
                indeces.Add(tRI);
                indeces.Add(bRI);

                outlines.Add(tI);
                outlines.Add(lI);
            break;
            case 12:
                b = CrossPos(bR.value,bRpos,bL.value,bLpos,isoValue);
                t = CrossPos(tR.value,tRpos,tL.value,tLpos,isoValue);

                bI = assignIndex(b);
                tI = assignIndex(t);
                bLI = assignIndex(bLpos);
                tLI = assignIndex(tLpos);

                indeces.Add(bLI);
                indeces.Add(tLI);
                indeces.Add(tI);

                indeces.Add(bLI);
                indeces.Add(tI);
                indeces.Add(bI);

                outlines.Add(bI);
                outlines.Add(tI);
                break;
            case 13:
                r = CrossPos(tR.value,tRpos,bR.value,bRpos,isoValue);
                t = CrossPos(tR.value,tRpos,tL.value,tLpos,isoValue);

                rI = assignIndex(r);
                tI = assignIndex(t);
                bLI = assignIndex(bLpos);
                tLI = assignIndex(tLpos);
                bRI = assignIndex(bRpos);

                indeces.Add(bLI);
                indeces.Add(tLI);
                indeces.Add(tI);

                indeces.Add(bLI);
                indeces.Add(tI);
                indeces.Add(rI);

                indeces.Add(bLI);
                indeces.Add(rI);
                indeces.Add(bRI);

                outlines.Add(rI);
                outlines.Add(tI);
            break;
            case 14:
                b = CrossPos(bR.value,bRpos,bL.value,bLpos,isoValue);
                r = CrossPos(bR.value,bRpos,tR.value,tRpos,isoValue);

                rI = assignIndex(r);
                bI = assignIndex(b);
                tLI = assignIndex(tLpos);
                bLI = assignIndex(bLpos);
                tRI = assignIndex(tRpos);

                indeces.Add(tLI);
                indeces.Add(bI);
                indeces.Add(bLI);

                indeces.Add(tLI);
                indeces.Add(rI);
                indeces.Add(bI);

                indeces.Add(tLI);
                indeces.Add(tRI);
                indeces.Add(rI);

                outlines.Add(bI);
                outlines.Add(rI);
            break;
            case 15:
                // all wall case: isoContour on GridPoints

                bLI = assignIndex(bLpos);
                tLI = assignIndex(tLpos);
                tRI = assignIndex(tRpos);
                bRI = assignIndex(bRpos);

                indeces.Add(bLI);
                indeces.Add(tLI);
                indeces.Add(tRI);

                indeces.Add(bLI);
                indeces.Add(tRI);
                indeces.Add(bRI);
            break;
        }
    }
}
