using UnityEngine;

[System.Serializable]
public class MapObjectData
{
    public string filename;
    public MapObjectType type;
}

[CreateAssetMenu(fileName = "MapObjectTable", menuName = "GameTable/MapObjectTable")]
public class MapObjectTable : ScriptableObject
{
    public MapObjectData[] lists;
}