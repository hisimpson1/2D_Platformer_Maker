using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ItemIndexButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public EventManager eventManager;
    public GameObject prefab;  // 월드에 설치할 프리팹
    public Camera mainCamera;
    public ObjectSelectionType objectSelectionType;

    private CanvasGroup canvasGroup;
    private GameObject previewInstance;
    private RectTransform rectTransform;
    private Canvas canvas;


    public int index;
    private HorizontalScrollController controller;
    

    void Awake()
    {
        // 부모에서 컨트롤러 찾기
        controller = GetComponentInParent<HorizontalScrollController>();
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnClick()
    {
        if(controller)
            controller.HandleItemButtonClick(index);
    }

    GameObject SpawnDragSpriteObject(Sprite sprite, Vector3 worldPosition)
    {
        GameObject go = new GameObject("DragSpriteObject");
        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;           

        // 위치, 크기, sorting 등 설정
        go.transform.position = worldPosition;
        go.transform.localScale = Vector3.one;  // 필요시 크기 조정
        renderer.sortingOrder = 2;
        return go;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.7f;

        // ScrollView 스크롤 막기 (중요!)
        ScrollRect scrollRect = GetComponentInParent<ScrollRect>();
        if (scrollRect != null) 
            scrollRect.enabled = false;

        // Preview 생성 (월드에)
        if (objectSelectionType == ObjectSelectionType.TileMap || objectSelectionType == ObjectSelectionType.MapObject)
        {
            string folderName = FileUtil.GetObjectSelectionFolderName(objectSelectionType);
            TileBase tile = TileBaseManager.Instance.GetTileByIndex(folderName, index);
            if (tile is Tile sourceTile)
            {
                previewInstance = SpawnDragSpriteObject(sourceTile.sprite, mainCamera.transform.position);
            }
        }
        else if (objectSelectionType == ObjectSelectionType.Actor)
        {
            if (prefab == null)
                return;

            previewInstance = Instantiate(prefab, mainCamera.transform.position, Quaternion.identity);

            Rigidbody2D rgidbody = previewInstance.gameObject.GetComponent<Rigidbody2D>();
            if (rgidbody)
            {
                rgidbody.simulated = false;
            }
        }

        previewInstance.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
        previewInstance.GetComponent<SpriteRenderer>().sortingOrder = 1;

        previewInstance.transform.localScale *= 0.8f;  // 미리보기 크기 조절
        MakePreviewTransparent(previewInstance);       // 투명하게

        canvasGroup.blocksRaycasts = false;  // UI 드래그 중 클릭 막기
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (previewInstance == null) return;
        // UI 아이템의 원본 따라다니기 (필요시)
        //rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        // Preview 월드 위치 업데이트 (Raycast로 설치 가능 위치 찾기)
        UpdatePreviewPosition(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (previewInstance == null) return;

        canvasGroup.alpha = 1.0f;
        ScrollRect scrollRect = GetComponentInParent<ScrollRect>();
        if (scrollRect != null) 
            scrollRect.enabled = true;
        //rectTransform.anchoredPosition = Vector2.zero;  //OnDrag에서 rectTransform.anchoredPosition이 수정될때만 사용

        Vector2 screenPos = eventData.position;
        Vector2 worldPos = GetWorldInstallPosition(screenPos);
                                         
        //Instantiate(prefab, installPos, Quaternion.identity);
        eventManager.TriggerPlaceDragObject(objectSelectionType, index, worldPos);

        Destroy(previewInstance);
        canvasGroup.blocksRaycasts = true;
        //rectTransform.anchoredPosition = Vector2.zero;  // 원위치
    }

    private void UpdatePreviewPosition(Vector2 screenPos)
    {
        Vector3 worldPos = GetWorldInstallPosition(screenPos);
        previewInstance.transform.position = worldPos;
    }
    private Vector2 GetWorldInstallPosition(Vector2 screenPos)
    {
        return mainCamera.ScreenToWorldPoint(screenPos);
    }

    private void MakePreviewTransparent(GameObject obj)
    {
        SpriteRenderer sr = obj.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = 0.5f;
            sr.color = c;
        }
    }

}
