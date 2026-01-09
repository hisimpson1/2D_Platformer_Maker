using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Tilemaps;
using UnityEngine.Timeline;

public static class FileUtil
{
    public static void AppendTilemapList (GamemapList saveData, Tilemap tilemap, string folderName)
    {
        BoundsInt bounds = tilemap.cellBounds;
        foreach (var pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile != null)
            {
                saveData.tiles.Add(new TileData
                {
                    x = pos.x,
                    y = pos.y,
                    tileName = tile.name,
                    folderName = folderName
                });
            }
        }
    }


    public static void SaveMap(GamemapList saveData)
    {
        string json = JsonUtility.ToJson(saveData);
        string path = Path.Combine(Application.persistentDataPath, "gamemap.json");
        File.WriteAllText(path, json);

        Debug.Log("Tilemap saved to: " + path);
    }

    public static GamemapList LoadMap()
    {
        string path = Path.Combine(Application.persistentDataPath, "gamemap.json");
        if (!File.Exists(path))
        {
            Debug.LogWarning("No load file found at: " + path);
            return new GamemapList();
        }

        string json = File.ReadAllText(path);
        GamemapList loadData = JsonUtility.FromJson<GamemapList>(json);

        Debug.Log("gamemap.json loaded from: " + path);
        return loadData;
    }

    public static void ReadTileMapList(GamemapList gamemapList, Tilemap tilemap, Dictionary<string, TileBase> tileDict, string folderName)
    {
        if (tileDict.Count == 0)
            return;

        tilemap.ClearAllTiles();

        foreach (var data in gamemapList.tiles)
        {
            if (folderName != data.folderName)
                continue;

            Vector3Int pos = new Vector3Int(data.x, data.y, 0);
            if (tileDict.TryGetValue(data.tileName, out TileBase tile))
            {
                tilemap.SetTile(pos, tile);
            }
        }
    }

    public static void ListFiles()
    {
        string path = Application.persistentDataPath;

        if (Directory.Exists(path))
        {
            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                Debug.Log("File: " + file);
            }
        }
    }

    public static string GetObjectSelectionFolderName(ObjectSelectionType selectionType)
    {
        string folderName = "";

        switch (selectionType)
        {
            case ObjectSelectionType.TileMap: folderName = "TileMap"; break;
            case ObjectSelectionType.MapObject: folderName = "MapObject"; break;
            case ObjectSelectionType.Actor: folderName = "Actor"; break;
        }

        return folderName;
    }
}
