using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TileBaseManager : MonoBehaviour
{
    public static TileBaseManager Instance { get; private set; }

    // 폴더별 Dictionary와 이름 리스트를 함께 관리
    private Dictionary<string, Dictionary<string, TileBase>> folderTileDict = new Dictionary<string, Dictionary<string, TileBase>>();
    private Dictionary<string, List<string>> folderTileNames  = new Dictionary<string, List<string>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// 특정 폴더에서 타일 로드
    /// </summary>
    public void LoadTilesFromFolder(string folderName)
    {
        if (folderTileDict.ContainsKey(folderName)) return; // 이미 로드됨

        TileBase[] tiles = Resources.LoadAll<TileBase>(folderName);
        var dict = new Dictionary<string, TileBase>();
        var names = new List<string>();

        foreach (TileBase tile in tiles)
        {
            if (tile != null)
            {
                dict[tile.name] = tile;
                names.Add(tile.name);
                Debug.Log($"Tile 로드됨: {tile.name} (폴더: {folderName})");
            }
        }

        folderTileDict[folderName] = dict;
        folderTileNames[folderName] = names;

        Debug.Log($"'{folderName}' 폴더의 타일 로딩 완료!");
    }

    /// <summary>
    /// 폴더 + 이름으로 TileBase 가져오기
    /// </summary>
    public TileBase GetTile(string folderName, string tileName)
    {
        if (GetTileCount(folderName) == 0)
            TileBaseManager.Instance.LoadTilesFromFolder(folderName);

        if (folderTileDict.TryGetValue(folderName, out var dict))
        {
            if (dict.TryGetValue(tileName, out TileBase tile))
                return tile;
        }

        Debug.LogWarning($"Tile '{tileName}'을 '{folderName}' 폴더에서 찾을 수 없습니다.");
        return null;
    }

    /// <summary>
    /// 폴더 + 인덱스로 TileBase 가져오기
    /// </summary>
    public TileBase GetTileByIndex(string folderName, int index)
    {
        if (GetTileCount(folderName) == 0)
            TileBaseManager.Instance.LoadTilesFromFolder(folderName);

        if (folderTileNames.TryGetValue(folderName, out var names))
        {
            if (index >= 0 && index < names.Count)
            {
                string name = names[index];
                return GetTile(folderName, name);
            }
            Debug.LogWarning($"인덱스 {index}가 '{folderName}' 폴더 범위를 벗어났습니다.");
        }
        else
        {
            Debug.LogWarning($"'{folderName}' 폴더는 아직 로드되지 않았습니다.");
        }
        return null;
    }

    /// <summary>
    /// 폴더별 전체 타일 개수
    /// </summary>
    public int GetTileCount(string folderName)
    {
        List<string> names;
        if (folderTileNames.TryGetValue(folderName, out names))
            return names.Count;

        TileBaseManager.Instance.LoadTilesFromFolder(folderName);
        if (folderTileNames.TryGetValue(folderName, out names))
            return names.Count;

        Debug.LogWarning($"'{folderName}' 폴더는 아직 로드되지 않았습니다.");
        return 0;
    }

    public Dictionary<string, TileBase> GetTileDictionary(string folderName)
    {
        if (folderTileDict.TryGetValue(folderName, out var dict))
            return dict;

        // 폴더가 없을 경우 처리
        Debug.LogWarning($"'{folderName}' 폴더는 아직 로드되지 않았습니다.");
        return new Dictionary<string, TileBase>();
    }
}