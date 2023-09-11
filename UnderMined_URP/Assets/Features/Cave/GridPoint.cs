using UnityEngine;

public struct GridPoint
{
    /// <summary> world space pos of grid point</summary>
    public Vector3 pos;

    /// <summary> SignedDistanceField Value of this GridPoint </summary>
    public float value;



    public WallType wallType;

    public GridPoint(Vector3 _pos)
    {
        pos = _pos;
        value = 1f;
        wallType = WallType.Stone;
    }
    
    public GridPoint(Vector3 _pos, float _value)
    {
        pos = _pos;
        value = _value;
        wallType = WallType.Stone;
    }
    
    public GridPoint(Vector3 _pos, float _value, WallType _wallType)
    {
        pos = _pos;
        value = _value;
        wallType = _wallType;
    }
}