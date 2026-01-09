using UnityEngine;


public class CameraCenterFollow : MonoBehaviour
{
    public Transform player;       // 따라갈 플레이어
    public float smoothTime = 0.3f; // 카메라가 목표 위치에 도달하는 데 걸리는 시간
    private Vector3 velocity = Vector3.zero; // SmoothDamp에서 내부적으로 사용하는 속도 값

    public float smoothSpeed = 0.125f; // 부드럽게 따라오는 속도
    private Vector3 offset;         //자동으로 계산되는 오프셋

    public bool smoothDamp = true;

    void Start()
    {
        // 카메라와 플레이어의 초기 상대 위치를 자동으로 계산
        offset = transform.position - player.position;
    }

    void LateUpdate()
    {
        if (smoothDamp)
            MoveBySmoothDamp();
        else
            MoveByFixedVelocity();
    }

    void MoveBySmoothDamp()
    {
        // 목표 위치 = 플레이어 위치 + 오프셋
        Vector3 targetPosition = player.position + offset;

        // SmoothDamp로 부드럽게 이동
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    void MoveByFixedVelocity()
    {
        // 목표 위치 = 플레이어 위치 + 오프셋
        Vector3 desiredPosition = player.position + offset;
        // 부드럽게 이동
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;
    }
}