using UnityEngine;

abstract class Water : MonoBehaviour
{
    TerrainType terrainType = TerrainType.Water;
    private int _height;
    public virtual int Height
    {
        get
        {
            return _height;
        }
    }

    public virtual void ChangeHeight(int newHeight)
    {
        _height = newHeight;
    }
}


