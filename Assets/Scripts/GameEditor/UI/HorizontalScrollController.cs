using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class HorizontalScrollController : MonoBehaviour
{
    private ScrollRect scrollRect;
    private RectTransform content;

    private float space = 1;

    public GameObject itemPrefab;

    public List<GameObject> itemList = new List<GameObject>();

    private ObjectSelectionType objectSelectionType = ObjectSelectionType.TileMap;

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();

        if (scrollRect != null)
            content = scrollRect.content;
    }

    public void SetObjectSelectionType(ObjectSelectionType selectionType)
    {
        objectSelectionType = selectionType;
    }

    public ObjectSelectionType GetObjectSelectionType()
    {
        return objectSelectionType;
    }

    void Start()
    {
        ChangeScrollViewContent();
    }

    public void ChangeScrollViewContent()
    {
        ClearAllItems();

        if (objectSelectionType == ObjectSelectionType.Actor)
            ChangeScrollViewActor();
        else
            ChangeScrollViewTile();
    }

    void ChangeScrollViewActor()
    {
        ActorSpriteData[] actorTable = ActorTableManager.Instance.GetAllActors();
        for (int n = 0; n < actorTable.Count(); n++)
        {
            AddItem();
        }

        for (int n = 0; n < itemList.Count; n++)
        {
            Image image = itemList[n].GetComponentInChildren<Image>();
            image.sprite = actorTable[n].sprite;

            Button btn = itemList[n].GetComponentInChildren<Button>();
            Text btnText = itemList[n].GetComponentInChildren<Text>();
            btnText.text = actorTable[n].name;

            ItemIndexButton itemIndex = itemList[n].GetComponent<ItemIndexButton>();
            itemIndex.index = n;
            itemIndex.mainCamera = GameReferences.Instance.modeManager.editorCamera;
            itemIndex.objectSelectionType = objectSelectionType;
            itemIndex.prefab = GameReferences.Instance.GetActorPrefab(n);
            itemIndex.eventManager = EventManager.Instance;

            btn.onClick.AddListener(itemIndex.OnClick);
        }

        SetSpacing(space);
        UpdateContentWidth();
    }

    void ChangeScrollViewTile()
    {
        string folderName = FileUtil.GetObjectSelectionFolderName(objectSelectionType);
        for (int i = 0; i < TileBaseManager.Instance.GetTileCount(folderName); i++)
        {
            AddItem();
        }

        for (int i = 0; i < itemList.Count; i++)
        {
            Image image = itemList[i].GetComponentInChildren<Image>();
            TileBase tile = TileBaseManager.Instance.GetTileByIndex(folderName, i);
            // Tile로 형변환하여 Sprite 정보가 있는지 확인
            if (tile is Tile sourceTile)
            {
                image.sprite = sourceTile.sprite;
                image.color = sourceTile.color;
            }

            Button btn = itemList[i].GetComponentInChildren<Button>();
            Text btnText = itemList[i].GetComponentInChildren<Text>();
            btnText.text = (i + 1).ToString();

            ItemIndexButton itemIndex = itemList[i].GetComponent<ItemIndexButton>();
            itemIndex.index = i;
            itemIndex.mainCamera = GameReferences.Instance.modeManager.editorCamera;
            itemIndex.objectSelectionType = objectSelectionType;
            itemIndex.eventManager = EventManager.Instance;

            btn.onClick.AddListener(itemIndex.OnClick);
        }

        SetSpacing(space);
        UpdateContentWidth();
    }

    public void HandleItemButtonClick(int index)
    {
        //Debug.Log("버튼 클릭 index: " + index);
        if (objectSelectionType == ObjectSelectionType.Actor) 
        {
            EventManager.Instance.TriggerActorSelected(index);
        }
        else
        {
            //string folderName = FileUtil.GetObjectSelectionFolderName(objectSelectionType);
            //TileBase tileBase = TileBaseManager.Instance.GetTileByIndex(folderName, index);
            //DDD
            //EventManager.Instance.TriggerTileBaseSelected(tileBase);
            EventManager.Instance.TriggerTileBaseSelected(objectSelectionType, index);
        }
    }
    public void AddItem()
    {
        GameObject newUI = Instantiate(itemPrefab, content);
        itemList.Add(newUI);
    }

    public void UpdateContentWidth()
    {
        // 1. 필요한 컴포넌트 정보 가져오기
        HorizontalLayoutGroup layoutGroup = content.GetComponent<HorizontalLayoutGroup>();
        RectTransform contentRect = content.GetComponent<RectTransform>();

        int childCount = content.childCount;

        if (childCount == 0) return;

        // 2. 아이템 하나의 가로 길이 (첫 번째 자식 기준)
        float itemWidth = content.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;

        // 3. 전체 너비 계산 공식:
        // (아이템 너비 * 개수) + (사이 간격 * (개수 - 1)) + 좌우 패딩
        float totalWidth = (itemWidth * childCount)
                           + (layoutGroup.spacing * (childCount - 1))
                           + layoutGroup.padding.left
                           + layoutGroup.padding.right;

        // 4. Content의 sizeDelta 수정 (가로 길이 반영)
        contentRect.sizeDelta = new Vector2(totalWidth, contentRect.sizeDelta.y);
    }

    public void ClearAllItems()
    {
        foreach (var item in itemList)
        {
            Destroy(item);
        }
        itemList.Clear();
    }

    public void SetSpacing(float spacing)
    {
        HorizontalLayoutGroup layout = content.GetComponent<HorizontalLayoutGroup>();
        if (layout != null)
        {
            layout.spacing = spacing;

            // 간격이 바뀌면 전체크기가 다시 계산 되어야 하므로 UpdateContentWidth를 실행한다.
            LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        }
    }
}