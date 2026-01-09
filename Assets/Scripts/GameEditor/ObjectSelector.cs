using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;  // ← 이거 추가!

public class ObjectSelector : MonoBehaviour
{
    [SerializeField] private Camera CurrentCamera;
    [SerializeField] private Material yellowSilhouetteMaterial;

    private Transform actorRoot;
    private GameObject selectedObject;
    private ToggleSelector toggleSelector;

    // 선택된 타일 정보 저장
    private Tilemap platformTilemap;
    private Tilemap mapObjectTilemap;
    private Vector3Int selectedCellPos;
    private TileBase selectedTile;

    private LineRenderer tileHighlightLine;

    void Awake()
    {
        toggleSelector = GetComponentInChildren<ToggleSelector>();
    }

    void Start()
    {
        platformTilemap = GameReferences.Instance.GetTilemap();
        mapObjectTilemap = GameReferences.Instance.GetMapObjectTilemap();

        actorRoot = GameReferences.Instance.GetActorRoot().transform;

        EventManager.Instance.OnObjectSelectionType += HandleSelectionObjectType;
        EventManager.Instance.OnRemoveEndDragObject += HandleRemoveEndDragObject;
    }

    void OnDestroy()
    {
        EventManager.Instance.OnObjectSelectionType -= HandleSelectionObjectType;
        EventManager.Instance.OnRemoveEndDragObject -= HandleRemoveEndDragObject;
    }

    void Update()
    {
        // ← 여기 수정! UI 체크 추가
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            SelectObjectAtMouse();
        }
    }

    public Vector3Int GetSelectedCellPos()
    {
        return selectedCellPos;
    }

    public GameObject GetSelectedObject()
    {
        return selectedObject;
    }


    // UI 위인지 체크하는 헬퍼 메서드 (추천: 한 번만 호출)
    bool IsPointerOverUI()
    {
        // 마우스
        if (EventSystem.current && EventSystem.current.IsPointerOverGameObject())
            return true;

        // 터치 (모바일 대응)
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                    return true;
            }
        }

        return false;
    }

    void SelectObjectAtMouse()
    {
        ObjectSelectionType selectionType = toggleSelector.GetSelectionType();
        if (selectionType == ObjectSelectionType.TileMap)
        {
            SelectTileMap(platformTilemap);
        }
        else if (selectionType == ObjectSelectionType.MapObject)
        {
            SelectTileMap(mapObjectTilemap);
        }
        if (selectionType == ObjectSelectionType.Actor)
        {
            SelectObjectActor();
        }
    }

    void SelectTileMap(Tilemap tilemap)
    {
        Vector3 mouseWorldPos = CurrentCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector3Int cellPos = tilemap.WorldToCell(mouseWorldPos);
        TileBase tile = tilemap.GetTile(cellPos);

        if (tile != null)
        {
            selectedCellPos = cellPos;
            selectedTile = tile;

            // 라인 박스 갱신
            UpdateLineBox(cellPos, tilemap);
            tileHighlightLine.enabled = true;

            Debug.Log($"Tile 선택됨: {tile.name}, CellPos: {cellPos}");
        }
        else
        {
            selectedTile = null;
            ClearLineBox();
            Debug.Log("선택된 타일 없음");
        }
    }

    void SelectObjectActor()
    {
        Vector3 mouseWorldPos = CurrentCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, 0f, -1);

        DeselectPrevious();

        if (hit.collider != null && hit.collider.transform.IsChildOf(actorRoot))
        {
            selectedObject = hit.collider.gameObject;
            HighlightSelected(selectedObject);
            Debug.Log($"선택됨: {selectedObject.name}");
        }
        else
        {
            selectedObject = null;
        }
    }

    void DeselectPreviousColor()
    {
        if (selectedObject != null)
        {
            var renderer = selectedObject.GetComponent<SpriteRenderer>();
            if (renderer) renderer.color = Color.white;
        }
    }

    void HighlightSelectedColor(GameObject obj)
    {
        var renderer = obj.GetComponent<SpriteRenderer>();
        if (renderer) renderer.color = Color.yellow;
    }

    void HighlightSelected(GameObject obj)
    {
        if (!obj) return;

        // 없으면 자동 추가
        DuplicateOutlineSprite outline = obj.GetComponent<DuplicateOutlineSprite>();
        if (!outline)
            outline = obj.AddComponent<DuplicateOutlineSprite>();

        // Inspector에서 연결한 Material 할당
        outline.yellowSilhouetteMaterial = yellowSilhouetteMaterial;
        outline.EnsureOutlineCreated();
        outline.SetOutlineActive(true);
    }

    public void DeselectPrevious()
    {
        if (!selectedObject) 
            return;

        // Remove Component
        var outline = selectedObject.GetComponent<DuplicateOutlineSprite>();
        if (outline)
        {
            outline.SetOutlineActive(false);
            Destroy(outline);
        }
    }

    void HandleSelectionObjectType(ObjectSelectionType selectionType)
    {
        ResetSelectMark();
    }

    void HandleRemoveEndDragObject()
    {
        ResetSelectMark();
    }

    void ResetSelectMark()
    {
        DeselectPrevious();
        selectedObject = null;
        ClearLineBox();
        selectedTile = null;
    }

    void CreateLineBox(Tilemap tilemap)
    {
        if (tileHighlightLine == null)
        {
            GameObject lineObj = new GameObject("TileLineHighlight");
            tileHighlightLine = lineObj.AddComponent<LineRenderer>();
            tileHighlightLine.positionCount = 5; // 네 모서리 + 시작점
            tileHighlightLine.loop = false;
            tileHighlightLine.widthMultiplier = 0.05f;
            tileHighlightLine.material = new Material(Shader.Find("Sprites/Default"));
            tileHighlightLine.startColor = Color.yellow;
            tileHighlightLine.endColor = Color.yellow;
        }
    }

    void UpdateLineBox(Vector3Int cellPos, Tilemap tilemap)
    {
        CreateLineBox(tilemap);

        Vector3 cellWorldPos = tilemap.GetCellCenterWorld(cellPos);
        Vector3 cellSize = tilemap.cellSize;

        Vector3 halfSize = cellSize * 0.5f;
        Vector3[] corners = new Vector3[5];
        corners[0] = cellWorldPos + new Vector3(-halfSize.x, -halfSize.y, 0);
        corners[1] = cellWorldPos + new Vector3(-halfSize.x, halfSize.y, 0);
        corners[2] = cellWorldPos + new Vector3(halfSize.x, halfSize.y, 0);
        corners[3] = cellWorldPos + new Vector3(halfSize.x, -halfSize.y, 0);
        corners[4] = corners[0]; // 닫기

        tileHighlightLine.SetPositions(corners);
    }

    void ClearLineBox()
    {
        if (tileHighlightLine != null)
        {
            tileHighlightLine.enabled = false; // Destroy 대신 비활성화
        }
    }

}