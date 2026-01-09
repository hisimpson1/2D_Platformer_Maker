using UnityEngine;


public class CameraFollow : MonoBehaviour
{
    public Transform targetTransform;       // 따라갈 플레이어
    public Rigidbody2D targetRigid;
    float smoothTime = 0.3f; // 카메라가 목표 위치에 도달하는 데 걸리는 시간
    private Vector3 velocity = Vector3.zero; // SmoothDamp에서 내부적으로 사용하는 속도 값

    float smoothSpeed = 0.125f; // 부드럽게 따라오는 속도
    private Vector3 baseOffset;         //자동으로 계산되는 오프셋
    private int lastDirection = 1; // 마지막 이동 방향 (1 = 오른쪽, -1 = 왼쪽)

    // 맵 경계 자동 계산용
    Collider2D mapBounds; // 맵 전체를 감싸는  Collider2D (예: BoxCollider2D)
    private float minX, maxX, minY, maxY;
    private Camera cam;

    // 여유 공간 (맵 끝에서 조금 더 보여주기)
    float marginX = 0.1f;
    float marginY = 30f;

    public bool smoothDamp = true;

    void Start()
    {
        // 카메라와 플레이어의 초기 상대 위치를 자동으로 계산
        baseOffset = transform.position - targetTransform.position;

        if (GameReferences.Instance.GetTilemap())
        {
            mapBounds = GameReferences.Instance.GetTilemap().GetComponent<Collider2D>();
        }
        SetCameraBound();
    }

    void OnEnable()
    {
        SetCameraBound();
    }

    void OnDisable()
    {
    }

    void SetMapBounds(Collider2D map)
    {
        mapBounds = map;
    }

    void SetCameraBound()
    {
        if (mapBounds == null)
            return;

        cam = Camera.main;

        // 카메라 크기 계산
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;
        // 맵 경계 가져오기
        Bounds bounds = mapBounds.bounds;

        // 카메라가 맵 밖으로 나가지 않도록 제한값 계산 + margin 추가
        if (bounds.size.x < camWidth * 2)
        {
            minX = maxX = bounds.center.x;
        }
        else
        {
            minX = bounds.min.x + camWidth - marginX;
            maxX = bounds.max.x - camWidth + marginX;
        }

        minY = bounds.min.y + camHeight - marginY;
        maxY = bounds.max.y - camHeight + marginY;
    }

    void LateUpdate()
    {
        if (smoothDamp)
            MoveBySmoothDamp();
        else
            MoveByFixedVelocity();
    }

    Vector3 GetTargetPosition()
    {
        // 기본
        // 목표 위치 = 플레이어 위치 + 오프셋
        //Vector3 targetPosition = targetTransform.position + baseOffset;

        // 플레이어의 이동 속도 확인
        float moveDir = targetRigid.linearVelocity.x;

        // 이동 중이면 방향 갱신
        if (moveDir > 0.01f)
            lastDirection = 1; // 오른쪽
        else if (moveDir < -0.01f)
            lastDirection = -1; // 왼쪽 

        // 방향에 따라 오프셋 설정
        float xOffset = (lastDirection == 1) ? 3f : -2f;

        // 목표 위치 = 플레이어 위치 + 오프셋
        Vector3 targetPosition = new Vector3(targetTransform.position.x + xOffset, targetTransform.position.y + baseOffset.y, transform.position.z); // Z는 카메라 고정

        // 맵 경계 제한
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        //targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        return targetPosition;
    }

    void MoveBySmoothDamp()
    {
        Vector3 targetPosition = GetTargetPosition();
        // SmoothDamp로 부드럽게 이동
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    void MoveByFixedVelocity()
    {
        Vector3 targetPosition = GetTargetPosition();

        // 부드럽게 이동
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);

        transform.position = smoothedPosition;
    }
}
