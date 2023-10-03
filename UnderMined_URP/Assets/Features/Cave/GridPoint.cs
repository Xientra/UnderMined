using System;
using UnityEngine;

public struct GridPoint
{
    /// <summary> SignedDistanceField Value of this GridPoint. Values from 0-255; 255 means wall</summary>
    public byte value;

    public WallType wallType;
    
    public GridPoint(byte _value=255,WallType _wallType=WallType.Stone)
    {
        value = _value;
        wallType = _wallType;
    }

}
