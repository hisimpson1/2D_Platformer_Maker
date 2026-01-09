using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DuplicateOutlineSprite : MonoBehaviour
{
    [Header("Outline Settings")]
    public Material yellowSilhouetteMaterial;
    [SerializeField] private float scaleMultiplier = 1.3f;
    [SerializeField] private int sortingOrderOffset = -1;

    private GameObject outlineObject;
    private SpriteRenderer outlineRenderer;
    private SpriteRenderer sourceRenderer;
    private bool outlineCreated = false;

    void Awake()
    {
        sourceRenderer = GetComponent<SpriteRenderer>();
        if (yellowSilhouetteMaterial != null)
            CreateOutline();
        SetOutlineActive(false);
    }
    public void EnsureOutlineCreated()
    {
        if (!outlineCreated && yellowSilhouetteMaterial != null)
            CreateOutline();
    }

    void CreateOutline()
    {
        outlineCreated = true;
        if (!sourceRenderer) return;

        // 이미 있으면 중복 방지
        Transform existing = transform.Find("OutlineSprite");
        if (existing)
        {
            outlineObject = existing.gameObject;
            outlineRenderer = outlineObject.GetComponent<SpriteRenderer>();
            return;
        }

        // Outline Object 생성
        outlineObject = new GameObject("OutlineSprite");
        outlineObject.transform.SetParent(transform);
        outlineObject.transform.localPosition = Vector3.zero;
        outlineObject.transform.localRotation = Quaternion.identity;
        outlineObject.transform.localScale = Vector3.one * scaleMultiplier;

        outlineRenderer = outlineObject.AddComponent<SpriteRenderer>();
        outlineRenderer.sprite = sourceRenderer.sprite;

        // Sorting
        outlineRenderer.sortingLayerID = sourceRenderer.sortingLayerID;
        outlineRenderer.sortingOrder = sourceRenderer.sortingOrder + sortingOrderOffset;

        // flip 동기화
        outlineRenderer.flipX = sourceRenderer.flipX;
        outlineRenderer.flipY = sourceRenderer.flipY;

        // Mask 영향 안 받게
        outlineRenderer.maskInteraction = SpriteMaskInteraction.None;

        // 🔑 YellowSilhouette Material 적용
        if (yellowSilhouetteMaterial != null)
        {
            Material matInstance = new Material(yellowSilhouetteMaterial);

            // Sprite Texture 연결
            if (sourceRenderer.sprite != null && sourceRenderer.sprite.texture != null)
            {
                matInstance.SetTexture("_MainTex", sourceRenderer.sprite.texture);
            }

            outlineRenderer.material = matInstance;
        }
    }

    void LateUpdate()
    {
        if (!outlineRenderer || !sourceRenderer) return;

        // 애니메이션 Sprite 동기화
        outlineRenderer.sprite = sourceRenderer.sprite;
        outlineRenderer.flipX = sourceRenderer.flipX;
        outlineRenderer.flipY = sourceRenderer.flipY;
    }

    public void SetOutlineActive(bool active)
    {
        if (outlineObject)
            outlineObject.SetActive(active);
    }
}
