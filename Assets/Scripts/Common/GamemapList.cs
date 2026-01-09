using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileData
{
    public int x, y;
    public string tileName;
    public string folderName;
}

[System.Serializable]
public class ActorData
{
    public int prefabIndex;
    public Vector3 pos;
}

[System.Serializable]
public class GamemapList
{
    public List<TileData> tiles = new List<TileData>();
    public List<ActorData> actors = new List<ActorData>();
}
