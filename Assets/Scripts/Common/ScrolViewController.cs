using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewController : MonoBehaviour
{
    private ScrollRect scrollRect;

    public float space = 50;

    public GameObject uiPrefab;

    public List<RectTransform> uiObjects = new List<RectTransform>();

    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();

        AddItem();
        AddItem();
        AddItem();
        AddItem();
    }

    public void AddItem()
    {
        // ui 오브젝트 생성 및 위치 설정
        var newUI = Instantiate(uiPrefab, scrollRect.content).GetComponent<RectTransform>();
        uiObjects.Add(newUI);

        float x = 0f;
        for (int i = 0; i < uiObjects.Count; i++)
        {
            uiObjects[i].anchoredPosition = new Vector2(0f, -x);
            x += uiObjects[i].sizeDelta.x + space;
        }

        // 설정된 오브젝트 위치 기반 Content 크기 조정
        scrollRect.content.sizeDelta = new Vector2(x, scrollRect.content.sizeDelta.y);
    }
}