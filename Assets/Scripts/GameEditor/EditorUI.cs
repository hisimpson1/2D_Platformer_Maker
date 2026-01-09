using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


public class EditorUI : MonoBehaviour
{
    Tilemap platformTilemap;
    Tilemap mapObjectTilemap;
    public Tilemap trapTilemap;
    public TileBase tileToPlace;  // 추가할 타일
    Transform player;      // 플레이어 Transform 참조
    public Button buttonSave;
    public Button buttonLoad;
    public Button buttonRemoveObject;
    public HorizontalScrollController objectScrollView;

    private GameObject selectedPrefab;
    private ObjectSelector objectSelector;
    private ToggleSelector toggleSelector;

    void Awake()
    {
        objectSelector = GetComponent<ObjectSelector>();
        toggleSelector = GetComponentInChildren<ToggleSelector>();
    }

    void Start()
    {
        platformTilemap = GameReferences.Instance.GetTilemap();
        mapObjectTilemap = GameReferences.Instance.GetMapObjectTilemap();
        player = GameReferences.Instance.editPlayer.transform;
        buttonSave.onClick.AddListener(HandleSaveClick);
        buttonLoad.onClick.AddListener(HandleLoadClick);
        buttonRemoveObject.onClick.AddListener(HandleRemoveObjectClick);

        EventManager.Instance.OnTileBaseSelected += HandlePlaceTile;
        EventManager.Instance.OnActorSelected += HandlePlaceActor;
        EventManager.Instance.OnObjectSelectionType += HandleSelectionObjectType;
        EventManager.Instance.OnPlaceDragObject += HandlePlaceDragObject;
    }

    void OnDestroy()
    {
        EventManager.Instance.OnTileBaseSelected -= HandlePlaceTile;
        EventManager.Instance.OnActorSelected -= HandlePlaceActor;
        EventManager.Instance.OnObjectSelectionType -= HandleSelectionObjectType;
        EventManager.Instance.OnPlaceDragObject -= HandlePlaceDragObject;

        buttonSave.onClick.RemoveListener(HandleSaveClick);
        buttonLoad.onClick.RemoveListener(HandleLoadClick);
        buttonRemoveObject.onClick.RemoveListener(HandleRemoveObjectClick);
    }

    void Update()
    {
        // 스페이스바를 누르면 플레이어 위치에 타일 추가
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 플레이어의 월드 좌표 → 타일맵 좌표로 변환
            Vector3Int cellPos = platformTilemap.WorldToCell(player.position);

            // 해당 위치에 타일 배치
            //platformTilemap.SetTile(cellPos, tileToPlace);
            if (EditorManager.Instance)
                EditorManager.Instance.PlaceTile(platformTilemap, cellPos, tileToPlace);
        }

        // R 키를 누르면 플레이어 위치의 타일 제거
        if (Input.GetKeyDown(KeyCode.R))
        {
            Vector3Int cellPos = platformTilemap.WorldToCell(player.position);
            //platformTilemap.SetTile(cellPos, null); // null → 타일 제거
            if (EditorManager.Instance)
            {
                EditorManager.Instance.RemoveTile(platformTilemap, cellPos);
                EditorManager.Instance.RemoveTile(mapObjectTilemap, cellPos);
            }
        }
    }

    //void HandlePlaceTile(TileBase tileBase)
    void HandlePlaceTile(ObjectSelectionType selectionType, int index)
    {
        PlaceTile(selectionType, index, player.position);
    }

    void PlaceTile(ObjectSelectionType selectionType, int index, Vector3 pos)
    {
        string folderName = FileUtil.GetObjectSelectionFolderName(selectionType);
        TileBase tileBase = TileBaseManager.Instance.GetTileByIndex(folderName, index);

        Tilemap tilemap = null;
        if (selectionType == ObjectSelectionType.TileMap)
            tilemap = platformTilemap;
        else
            tilemap = mapObjectTilemap;

        if (tilemap == null)
        {
            Debug.LogWarning("tilemap is null. selectionType is :  " + selectionType);
            return;
        }

        // 플레이어의 월드 좌표 → 타일맵 좌표로 변환
        Vector3Int cellPos = tilemap.WorldToCell(pos);

        if (EditorManager.Instance)
        {
            EditorManager.Instance.PlaceTile(tilemap, cellPos, tileBase);
        }
    }

    void PlaceActor(int index, Vector3 pos)
    {
        GameObject prefab = GameReferences.Instance.GetActorPrefab(index);
        if (prefab && EditorManager.Instance)
        {
            EditorManager.Instance.PlaceActor(prefab, index, pos, GameReferences.Instance.GetActorRoot().transform);
        }
    }

    bool RemoveTile(ObjectSelectionType selectionType)
    {
        Tilemap tilemap = null;
        if (selectionType == ObjectSelectionType.TileMap)
            tilemap = platformTilemap;
        else if (selectionType == ObjectSelectionType.MapObject)
            tilemap = mapObjectTilemap;

        if (tilemap == null)
            return false;

        Vector3Int cellPos = objectSelector.GetSelectedCellPos();
        if (EditorManager.Instance)
            EditorManager.Instance.RemoveTile(tilemap, cellPos);

        return true;
    }

    void RemoveActor()
    {
        GameObject obj = objectSelector.GetSelectedObject();
        objectSelector.DeselectPrevious();

        if (obj != null)
        {
            GameObject actor = obj;
            if (EditorManager.Instance)
                EditorManager.Instance.RemoveActor(actor);
        }
    }

    void HandlePlaceActor(int index)
    {
        PlaceActor(index, player.position);
    }

    void HandleSelectionObjectType(ObjectSelectionType selectionType)
    {
        if (objectScrollView && selectionType != ObjectSelectionType.None)
        {
            objectScrollView.SetObjectSelectionType(selectionType);
            objectScrollView.ChangeScrollViewContent();
        }
    }

    void HandlePlaceDragObject(ObjectSelectionType selectionType, int index, Vector3 pos)
    {
        if (selectionType == ObjectSelectionType.TileMap || selectionType == ObjectSelectionType.MapObject)
        {
            PlaceTile(selectionType, index, pos);  
        } 
        else if (selectionType == ObjectSelectionType.Actor)
        {
            PlaceActor(index, pos);
        }
    }

    void AppendActorList(GamemapList saveData)
    {
        Transform actorRoot = GameReferences.Instance.GetActorRoot().transform;
        for (int n = 0; n < actorRoot.childCount; n++)
        {
            Transform trans = actorRoot.GetChild(n);
            ActorData data = new ActorData();

            var holder = trans.gameObject.GetComponent<IndexHolder>();
            if (holder != null)
            {
                data.prefabIndex = holder.index;
                data.pos = trans.position;
            }
            else
            {
                continue;
            }

            saveData.actors.Add(data);
        }
    }

    void ReadActorList(GamemapList loadData)
    {
        EngineUtil.ClearAllChildren(GameReferences.Instance.GetActorRoot());
        Transform actorRoot = GameReferences.Instance.GetActorRoot().transform;
        
        for (int n = 0; n < loadData.actors.Count; n++)
        {
            ActorData actorData = loadData.actors[n];
            GameObject prefab = GameReferences.Instance.GetActorPrefab(actorData.prefabIndex);
            if (prefab)
            {
                GameObject spawnedActor = Instantiate(prefab, actorData.pos, Quaternion.identity, actorRoot);
                var holder = spawnedActor.GetComponent<IndexHolder>();
                if (holder == null)
                    holder = spawnedActor.AddComponent<IndexHolder>();
                holder.index = actorData.prefabIndex;

                var sr = spawnedActor.GetComponent<SpriteRenderer>();
                if (sr)
                    sr.sortingOrder = 1;
            }
        }
    }

    void HandleSaveClick()
    {
        GamemapList saveData = new GamemapList();
        FileUtil.AppendTilemapList(saveData, platformTilemap, "TileMap");
        FileUtil.AppendTilemapList(saveData, mapObjectTilemap, "MapObject");
        AppendActorList(saveData);

        FileUtil.SaveMap(saveData);
    }

    void HandleLoadClick()
    {
        GamemapList loadData = FileUtil.LoadMap();

        string folderName = FileUtil.GetObjectSelectionFolderName(ObjectSelectionType.TileMap);
        TileBaseManager.Instance.LoadTilesFromFolder(folderName);
        FileUtil.ReadTileMapList(loadData, platformTilemap, TileBaseManager.Instance.GetTileDictionary(folderName), folderName);

        folderName = FileUtil.GetObjectSelectionFolderName(ObjectSelectionType.MapObject);
        TileBaseManager.Instance.LoadTilesFromFolder(folderName);
        FileUtil.ReadTileMapList(loadData, mapObjectTilemap, TileBaseManager.Instance.GetTileDictionary(folderName), folderName);

        ReadActorList(loadData);
    }

    void HandleRemoveObjectClick()
    {
        ObjectSelectionType selectionType = toggleSelector.GetSelectionType();
        if (selectionType == ObjectSelectionType.TileMap)
        {
            if (RemoveTile(selectionType) == false)
            {
                Vector3Int cellPos = platformTilemap.WorldToCell(player.position);
                if (EditorManager.Instance)
                    EditorManager.Instance.RemoveTile(platformTilemap, cellPos);
            }
        }
        else if (selectionType == ObjectSelectionType.MapObject)
        {
            if (RemoveTile(selectionType) == false)
            {
                Vector3Int cellPos = mapObjectTilemap.WorldToCell(player.position);
                if (EditorManager.Instance)
                    EditorManager.Instance.RemoveTile(mapObjectTilemap, cellPos);
            }
        }
        if (selectionType == ObjectSelectionType.Actor)
        {
            RemoveActor();
        }

        EventManager.Instance.TriggerRemoveEndDragObject();
    }
}