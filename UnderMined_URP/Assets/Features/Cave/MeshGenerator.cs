using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator
{
    public MeshInfo GenerateMeshFromMap(GridPoint[,] map, Dictionary<Vector3, int> gridPointDic, float isoValue) {
        int width = map.GetLength(0) - 1;
        int height = map.GetLength(1) - 1;
        // create squares in map
        GridSquare[,] squares = GenerateGridSquares(map, width, height);

        // growing list of triangle indeces
        List<int> indeces = new List<int>();

        // march through squares
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TriangulateSquare(squares[x,y], isoValue, gridPointDic, indeces);
            }
        }

        // vertex array
        Vector3[] vertices = new Vector3[gridPointDic.Keys.Count];
        // fill vertex array
        foreach(Vector3 key in gridPointDic.Keys) {
            vertices[gridPointDic[key]] = key;
        }

        int[] indecesArr = indeces.ToArray();

        return new MeshInfo(indecesArr, vertices);
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

    private void TriangulateSquare(GridSquare square, float isoValue, Dictionary<Vector3, int> GridPointDic, List<int> indeces) {
        
        /// <summary>
        /// linear interpolation to find position at which isoContour to the isoValue is
        /// </summary>
        /// <param name="lower"> is world space pos at the bottom left of the grid</param>
        /// <param name="higher"> is world space pos at the bottom left of the grid</param>
        /// <param name="isoValue"> is world space pos at the bottom left of the grid</param>
        Vector3 CrossPos(GridPoint lower, GridPoint higher, float isoValue) {
            Vector3 result = Vector3.zero;
            if(!lower.value.Equals(higher.value)) {
                float t = (isoValue - lower.value) / (higher.value - lower.value);
                result = (1-t) * lower.pos + t * higher.pos;
                if(float.IsNaN(result.x)) {
                    Debug.Log(lower.value + ", " + higher.value);
                }
            }
            return result;
        }
        
        GridPoint bL = square.bottomLeft;
        GridPoint tL = square.topLeft;
        GridPoint tR = square.topRight;
        GridPoint bR = square.bottomRight;

        // indeces of the grid points
        int bLI = GridPointDic[bL.pos];
        int tLI = GridPointDic[tL.pos];
        int tRI = GridPointDic[tR.pos];
        int bRI = GridPointDic[bR.pos];

        // index just counts upwards depending on length of gridpointDic
        int nextIndex = GridPointDic.Values.Count;

        // predefine possibly required iso contour poses
        Vector3 b = CrossPos(bL,bR,isoValue);
        Vector3 r = CrossPos(tR,bR,isoValue);
        Vector3 t = CrossPos(tL,tR,isoValue);
        Vector3 l = CrossPos(bL,tL,isoValue);

        /// <summary> if the vertex position has been added already, take the index of the existing vertex otherwise choose next bigger index and add it to dictionary </summary>
        int assignIndex(Vector3 pos) {
            int index;
            if(GridPointDic.TryGetValue(pos, out int value)) {
                index = value;
            } else {
                index = nextIndex++;
                GridPointDic.Add(pos, index);
            }
            return index;
        }

        // indeces of iso contour points
        int bI = assignIndex(b);
        int rI = assignIndex(r);
        int tI = assignIndex(t);
        int lI = assignIndex(l);

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
            b = CrossPos(bL,bR,isoValue);
            r = CrossPos(tR,bR,isoValue);

            indeces.Add(bI);
            indeces.Add(rI);
            indeces.Add(bRI);
            break;
            case 2:
            r = CrossPos(bR,tR,isoValue);
            t = CrossPos(tL,tR,isoValue);

            indeces.Add(rI);
            indeces.Add(tI);
            indeces.Add(tRI);
            break;
            case 3:
            b = CrossPos(bL,bR,isoValue);
            t = CrossPos(tL,tR,isoValue);

            indeces.Add(bI);
            indeces.Add(tRI);
            indeces.Add(bRI);

            indeces.Add(bI);
            indeces.Add(tI);
            indeces.Add(tRI);
            break;
            case 4:
            t = CrossPos(tR,tL,isoValue);
            l = CrossPos(bL,tL,isoValue);

            indeces.Add(lI);
            indeces.Add(tLI);
            indeces.Add(tI);
            break;
            case 5:
            l = CrossPos(bL,tL,isoValue);
            b = CrossPos(bL,bR,isoValue);

            r = CrossPos(tR,bR,isoValue);
            t = CrossPos(tR,tL,isoValue);

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
            break;
            case 6:
            l = CrossPos(bL,tL,isoValue);
            r = CrossPos(bR,tR,isoValue);

            indeces.Add(lI);
            indeces.Add(tLI);
            indeces.Add(tRI);

            indeces.Add(lI);
            indeces.Add(tRI);
            indeces.Add(rI);
            break;
            case 7:
            l = CrossPos(bL,tL,isoValue);
            b = CrossPos(bL,bR,isoValue);

            indeces.Add(lI);
            indeces.Add(tLI);
            indeces.Add(tRI);

            indeces.Add(lI);
            indeces.Add(tRI);
            indeces.Add(bI);

            indeces.Add(bI);
            indeces.Add(tRI);
            indeces.Add(bRI);
            break;
            case 8:
            l = CrossPos(tL,bL,isoValue);
            b = CrossPos(bR,bL,isoValue);

            indeces.Add(lI);
            indeces.Add(bI);
            indeces.Add(bLI);
            break;
            case 9:
            l = CrossPos(tL,bL,isoValue);
            r = CrossPos(tR,bR,isoValue);

            indeces.Add(lI);
            indeces.Add(rI);
            indeces.Add(bLI);

            indeces.Add(bLI);
            indeces.Add(rI);
            indeces.Add(bRI);
            break;
            case 10:
            t = CrossPos(tL,tR,isoValue);
            l = CrossPos(tL,bL,isoValue);

            b = CrossPos(bR,bL,isoValue);
            r = CrossPos(bR,tR,isoValue);

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
            break;
            case 11:
            t = CrossPos(tL,tR,isoValue);
            l = CrossPos(tL,bL,isoValue);

            indeces.Add(lI);
            indeces.Add(bRI);
            indeces.Add(bLI);

            indeces.Add(lI);
            indeces.Add(tI);
            indeces.Add(bRI);

            indeces.Add(tI);
            indeces.Add(tRI);
            indeces.Add(bRI);
            break;
            case 12:
            b = CrossPos(bR,bL,isoValue);
            t = CrossPos(tR,tL,isoValue);

            indeces.Add(bLI);
            indeces.Add(tLI);
            indeces.Add(tI);

            indeces.Add(bLI);
            indeces.Add(tI);
            indeces.Add(bI);
            break;
            case 13:
            r = CrossPos(tL,bR,isoValue);
            t = CrossPos(tR,tL,isoValue);

            indeces.Add(bLI);
            indeces.Add(tLI);
            indeces.Add(tI);

            indeces.Add(bLI);
            indeces.Add(tI);
            indeces.Add(rI);

            indeces.Add(bLI);
            indeces.Add(rI);
            indeces.Add(bRI);
            break;
            case 14:
            b = CrossPos(bR,bL,isoValue);
            r = CrossPos(bR,tR,isoValue);

            indeces.Add(tLI);
            indeces.Add(bI);
            indeces.Add(bLI);

            indeces.Add(tLI);
            indeces.Add(rI);
            indeces.Add(bI);

            indeces.Add(tLI);
            indeces.Add(tRI);
            indeces.Add(rI);
            break;
            case 15:
            // all wall caes: isoContour on GridPoints

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
