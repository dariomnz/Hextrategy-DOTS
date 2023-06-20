using Unity.Collections;
using Unity.Entities;
using Unity.Properties;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public partial struct HexCellTerrainData : IComponentData
{
    public HexTerrainType TerrainType;
    public int Elevation;
}

public struct HexCellTag : IComponentData { }

public struct HexCellRefresh : IComponentData, IEnableableComponent { }

public struct HexCellNeighborsEntity : IComponentData
{
    [SerializeField] private Entity NE;
    [SerializeField] private Entity E;
    [SerializeField] private Entity SE;
    [SerializeField] private Entity SW;
    [SerializeField] private Entity W;
    [SerializeField] private Entity NW;

    public HexCellNeighborsEntity(NativeArray<Entity> value)
    {
        NE = value[(int)HexDirection.NE];
        E = value[(int)HexDirection.E];
        SE = value[(int)HexDirection.SE];
        SW = value[(int)HexDirection.SW];
        W = value[(int)HexDirection.W];
        NW = value[(int)HexDirection.NW];
    }

    public Entity GetNeighbor(HexDirection i)
    {
        switch (i)
        {
            case HexDirection.NE:
                return NE;
            case HexDirection.E:
                return E;
            case HexDirection.SE:
                return SE;
            case HexDirection.SW:
                return SW;
            case HexDirection.W:
                return W;
            case HexDirection.NW:
                return NW;
        }
        return E;
    }
}

public struct HexCoordinates : IComponentData
{

    private int x, z;

    [SerializeField]
    public string Coords => ToString();

    public int X { get { return x; } set { x = value; } }
    public int Y { get { return -X - Z; } }
    public int Z { get { return z; } set { z = value; } }

    public HexCoordinates(int x, int z)
    {
        this.x = x - z / 2;
        this.z = z;
    }

    public override string ToString()
    {
        return "(" + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
    }
}

public struct HexCellConstData
{
    public const float xDiameter = 2f, zDiameter = 1.725f;
    public const float xRadius = xDiameter / 2f, zRadius = zDiameter / 2f;
}

public enum HexTerrainType
{
    None = -1,
    Water,
    Sand,
    Grass,
    Rock,
    Snow,
    Dirt,
}

public enum HexDirection
{
    NE, E, SE, SW, W, NW
}

public static class HexDirectionExtensions
{
    public static HexDirection Opposite(this HexDirection direction)
    {
        return (int)direction < 3 ? (direction + 3) : (direction - 3);
    }
    public static HexDirection Previous(this HexDirection direction)
    {
        return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
    }
    public static HexDirection Next(this HexDirection direction)
    {
        return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
    }
    public static HexDirection Previous2(this HexDirection direction)
    {
        direction -= 2;
        return direction >= HexDirection.NE ? direction : (direction + 6);
    }
    public static HexDirection Next2(this HexDirection direction)
    {
        direction += 2;
        return direction <= HexDirection.NW ? direction : (direction - 6);
    }
}