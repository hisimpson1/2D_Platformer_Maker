using System.Linq;
using UnityEngine;

public class MapObjectTableManager : MonoBehaviour
{
    public static MapObjectTableManager Instance { get; private set; }

    public MapObjectTable  table; 
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // 전체 배열을 반환
    public MapObjectData[] GetLists()
    {
        return table.lists;
    }

    public MapObjectType GetType(string name)
    {
        if (string.IsNullOrEmpty(name))
            return MapObjectType.None;

        var data = table.lists.FirstOrDefault(x => x.filename == name);
        return data != null ? data.type : MapObjectType.None;
    }
}