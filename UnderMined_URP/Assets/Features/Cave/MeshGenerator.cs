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
            return (1-isoValue) * lower.pos + isoValue * higher.pos;
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
        Vector3 b;
        Vector3 r;
        Vector3 t;
        Vector3 l;

        // indeces of iso contour points
        int bI;
        int rI;
        int tI;
        int lI;

        int caseNumber = 0;

        // adding the bit-value number based on pos in square
        if(bL.value > isoValue) {
            caseNumber += 8;
        }
        if(tL.value > isoValue) {
            caseNumber += 4;
        }
        if(bL.value > isoValue) {
            caseNumber += 2;
        }
        if(bL.value > isoValue) {
            caseNumber += 1;
        }

        switch(caseNumber) {
            case 0:
            // all empty case
            break;
            case 1:
            b = CrossPos(bL,bR,isoValue);
            r = CrossPos(tR,bR,isoValue);

            bI = nextIndex++;
            rI = nextIndex++;

            GridPointDic.Add(b,bI);
            GridPointDic.Add(r,rI);

            indeces.Add(bI);
            indeces.Add(bRI);
            indeces.Add(rI);
            break;
            case 2:
            r = CrossPos(bR,tR,isoValue);
            t = CrossPos(tL,tR,isoValue);

            rI = nextIndex++;
            tI = nextIndex++;

            GridPointDic.Add(r,rI);
            GridPointDic.Add(t,tI);

            indeces.Add(rI);
            indeces.Add(tRI);
            indeces.Add(tI);
            break;
            case 3:
            b = CrossPos(bL,bR,isoValue);
            t = CrossPos(tL,tR,isoValue);

            bI = nextIndex++;
            tI = nextIndex++;

            GridPointDic.Add(b,bI);
            GridPointDic.Add(t,tI);

            indeces.Add(bI);
            indeces.Add(bRI);
            indeces.Add(tRI);

            indeces.Add(bI);
            indeces.Add(tRI);
            indeces.Add(tI);
            break;
            case 4:
            t = CrossPos(tR,tL,isoValue);
            l = CrossPos(bL,tL,isoValue);

            tI = nextIndex++;
            lI = nextIndex++;

            GridPointDic.Add(t,tI);
            GridPointDic.Add(l,lI);

            indeces.Add(lI);
            indeces.Add(tI);
            indeces.Add(tLI);
            break;
            case 5:
            l = CrossPos(bL,tL,isoValue);
            b = CrossPos(bL,bR,isoValue);

            lI = nextIndex++;
            bI = nextIndex++;

            GridPointDic.Add(l,lI);
            GridPointDic.Add(b,bI);

            r = CrossPos(tR,bR,isoValue);
            t = CrossPos(tR,tL,isoValue);

            rI = nextIndex++;
            tI = nextIndex++;

            GridPointDic.Add(r,rI);
            GridPointDic.Add(t,tI);

            indeces.Add(tLI);
            indeces.Add(lI);
            indeces.Add(bI);

            indeces.Add(tLI);
            indeces.Add(bI);
            indeces.Add(tI);

            indeces.Add(tI);
            indeces.Add(bI);
            indeces.Add(bRI);

            indeces.Add(tI);
            indeces.Add(bRI);
            indeces.Add(rI);
            break;
            case 6:
            l = CrossPos(bL,tL,isoValue);
            r = CrossPos(bR,tR,isoValue);

            lI = nextIndex++;
            rI = nextIndex++;

            GridPointDic.Add(l,lI);
            GridPointDic.Add(r,rI);

            indeces.Add(lI);
            indeces.Add(tRI);
            indeces.Add(tLI);

            indeces.Add(lI);
            indeces.Add(rI);
            indeces.Add(tRI);
            break;
            case 7:
            l = CrossPos(bL,tL,isoValue);
            b = CrossPos(bL,bR,isoValue);

            lI = nextIndex++;
            bI = nextIndex++;

            GridPointDic.Add(l,lI);
            GridPointDic.Add(b,bI);

            indeces.Add(lI);
            indeces.Add(tRI);
            indeces.Add(tLI);

            indeces.Add(lI);
            indeces.Add(bI);
            indeces.Add(tRI);

            indeces.Add(bI);
            indeces.Add(bRI);
            indeces.Add(tRI);
            break;
            case 8:
            l = CrossPos(tL,bL,isoValue);
            b = CrossPos(bR,bL,isoValue);

            lI = nextIndex++;
            bI = nextIndex++;

            GridPointDic.Add(l,lI);
            GridPointDic.Add(b,bI);

            indeces.Add(lI);
            indeces.Add(bLI);
            indeces.Add(bI);
            break;
            case 9:
            l = CrossPos(tL,bL,isoValue);
            r = CrossPos(tR,bR,isoValue);

            lI = nextIndex++;
            rI = nextIndex++;

            GridPointDic.Add(l,lI);
            GridPointDic.Add(r,rI);

            indeces.Add(lI);
            indeces.Add(bLI);
            indeces.Add(rI);

            indeces.Add(bLI);
            indeces.Add(bRI);
            indeces.Add(rI);
            break;
            case 10:
            t = CrossPos(tL,tR,isoValue);
            l = CrossPos(tL,bL,isoValue);

            tI = nextIndex++;
            lI = nextIndex++;

            GridPointDic.Add(t,tI);
            GridPointDic.Add(l,lI);

            b = CrossPos(bR,bL,isoValue);
            r = CrossPos(bR,tR,isoValue);

            bI = nextIndex++;
            rI = nextIndex++;

            GridPointDic.Add(b,bI);
            GridPointDic.Add(r,rI);

            indeces.Add(lI);
            indeces.Add(bLI);
            indeces.Add(bI);

            indeces.Add(lI);
            indeces.Add(bI);
            indeces.Add(tI);

            indeces.Add(tI);
            indeces.Add(bI);
            indeces.Add(rI);

            indeces.Add(tI);
            indeces.Add(rI);
            indeces.Add(tRI);
            break;
            case 11:
            t = CrossPos(tL,tR,isoValue);
            l = CrossPos(tL,bL,isoValue);
            
            tI = nextIndex++;
            lI = nextIndex++;

            GridPointDic.Add(t,tI);
            GridPointDic.Add(l,lI);

            indeces.Add(lI);
            indeces.Add(bLI);
            indeces.Add(bRI);

            indeces.Add(lI);
            indeces.Add(bRI);
            indeces.Add(tI);

            indeces.Add(tI);
            indeces.Add(bRI);
            indeces.Add(tRI);
            break;
            case 12:
            b = CrossPos(bR,bL,isoValue);
            t = CrossPos(tR,tL,isoValue);

            bI = nextIndex++;
            tI = nextIndex++;

            GridPointDic.Add(b,bI);
            GridPointDic.Add(t,tI);

            indeces.Add(bLI);
            indeces.Add(tI);
            indeces.Add(tLI);

            indeces.Add(bLI);
            indeces.Add(bI);
            indeces.Add(tI);
            break;
            case 13:
            r = CrossPos(tL,bR,isoValue);
            t = CrossPos(tR,tL,isoValue);
            
            rI = nextIndex++;
            tI = nextIndex++;

            GridPointDic.Add(r,rI);
            GridPointDic.Add(t,tI);

            indeces.Add(bLI);
            indeces.Add(tI);
            indeces.Add(tLI);

            indeces.Add(bLI);
            indeces.Add(rI);
            indeces.Add(tI);

            indeces.Add(bLI);
            indeces.Add(bRI);
            indeces.Add(tI);
            break;
            case 14:
            b = CrossPos(bR,bL,isoValue);
            r = CrossPos(bR,tR,isoValue);

            bI = nextIndex++;
            rI = nextIndex++;

            GridPointDic.Add(b,bI);
            GridPointDic.Add(r,rI);

            indeces.Add(tLI);
            indeces.Add(bLI);
            indeces.Add(bI);

            indeces.Add(tLI);
            indeces.Add(bI);
            indeces.Add(rI);

            indeces.Add(tLI);
            indeces.Add(bI);
            indeces.Add(tRI);
            break;
            case 15:
            // all wall caes: isoContour on GridPoints
            indeces.Add(bLI);
            indeces.Add(tRI);
            indeces.Add(tLI);

            indeces.Add(tLI);
            indeces.Add(bRI);
            indeces.Add(tRI);
            break;
        }
    }
}
