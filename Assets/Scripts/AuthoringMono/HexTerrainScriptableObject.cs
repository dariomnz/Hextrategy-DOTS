using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;


[CreateAssetMenu(fileName = "HexTerrain", menuName = "ScriptableObjects/HexTerrain", order = 1)]
public class HexTerrainScriptableObject : ScriptableObject
{
    public SerializableDictionaryBase<HexTerrainType, Color> TerrainColors;
}