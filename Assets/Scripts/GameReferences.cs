using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameReferences : MonoBehaviour
{
    public static GameReferences Instance { get; private set; }

    public PlayerMove player;
    public EditorPlayerMove editPlayer;
    public GameObject[] actorPrefabs;
    public EditorManager editorManager;
    public ModeManager modeManager;
    public GameObject editorStage;
    private Tilemap platformTilemap;
    private Tilemap mapObjectTilemap;
    private GameObject actorRoot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Instance.InitObject();
        //DontDestroyOnLoad(gameObject);
    }

    void InitObject()
    {
        GameObject platformObject = GameObject.FindGameObjectWithTag("PlatformRoot");
        if (platformObject)
            platformTilemap = platformObject.GetComponent<Tilemap>();

        GameObject mapObject = GameObject.FindGameObjectWithTag("MapObjectRoot");
        if (mapObject)
            mapObjectTilemap = mapObject.GetComponent<Tilemap>();

        actorRoot = GameObject.FindGameObjectWithTag("ActorRoot");
    }

    public Tilemap GetTilemap()
    {
        return platformTilemap;
    }

    public Tilemap GetMapObjectTilemap()
    {
        return mapObjectTilemap;
    }

    public GameObject GetActorRoot()
    {
        return actorRoot;
    }

    private void Start()
    {
        ChangeMode(true);
    }

    public void ChangeMode(bool isPlayMode)
    {
        if (isPlayMode)
        {
            //if(editorStage)
            //    editorStage.gameObject.SetActive(false);  //TOTO: Later... 이걸 비활성화 하면 스테이지가 없어지네.. 방법을 찾자
            if (player && editPlayer)
            {
                player.gameObject.transform.position = editPlayer.gameObject.transform.position;
                player.gameObject.SetActive(true);
                editPlayer.gameObject.SetActive(false);
            }
        }
        else
        {
            if (editorStage)
                editorStage.gameObject.SetActive(true);
            if (player)
                player.gameObject.SetActive(false);
            if (editPlayer)
                editPlayer.gameObject.SetActive(true);
        }
    }

    public GameObject GetActorPrefab(int index)
    {
        if (index >= actorPrefabs.Count())
            return null;
        GameObject prefab = GameReferences.Instance.actorPrefabs[index];
        return prefab;
    }
}

