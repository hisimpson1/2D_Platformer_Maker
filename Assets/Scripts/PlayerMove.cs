using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.Tilemaps;

public class PlayerMove : MonoBehaviour
{
    [Header("--- References ---")]
    public GameManager gameManager;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capCollider;
    Animator anim;
    AudioSource audioSource;

    [Header("--- Movement Settings ---")]
    public float moveSpeed = 5f;      // 이동 속도
    public float jumpPower = 15f;    // 점프 힘
    public LayerMask platformLayer;   // 바닥 레이어 지정

    [Header("--- Slope Settings ---")]
    //디버깅을 위해 클래스 멤버로 선언
    private float slopeDownAngle;
    private Vector2 slopeNormalPerp; // 경사면에 평행한 방향 벡터
    private bool isOnSlope;


    [Header("--- Audio Clips ---")]
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;

    // 내부 변수
    float inputX;
    bool isGrounded;

    private GUIStyle debugLabelStyle;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capCollider = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        debugLabelStyle = new GUIStyle();
        debugLabelStyle.fontSize = 30; // 글자 크기 설정
        debugLabelStyle.fontStyle = FontStyle.Bold;
        debugLabelStyle.normal.textColor = Color.red;
    }

    void Update()
    {
        // 1. 입력 받기 (Update에서 받아야 입력 누락이 없음)
        inputX = Input.GetAxisRaw("Horizontal");

        // 2. 점프 시도
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        // 3. 스프라이트 방향 전환
        if (inputX != 0)
        {
            spriteRenderer.flipX = inputX == -1;
        }

        // 4. 애니메이션 상태 업데이트
        if (Mathf.Abs(rigid.linearVelocity.x) < 0.3f)
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);

        anim.SetBool("isJumping", !isGrounded);
    }

    void FixedUpdate()
    {
        CheckGround();
        CheckSlope(); // 경사면 체크 추가
        ApplyMovement(); // 이동 로직 분리
    }

    void CheckGround()
    {
        // 내 위치에서 아래로 레이를 쏨 (캐릭터의 발밑 지점 계산)
        float halfSize = capCollider.size.y / 2.0f;
        Vector2 rayOrigin = new Vector2(rigid.position.x, rigid.position.y - halfSize);
        RaycastHit2D rayHit = Physics2D.Raycast(rayOrigin, Vector2.down, halfSize*1.1f, platformLayer);

        // 레이가 바닥에 닿고 있으며, 떨어지는 중일 때만 바닥으로 간주
        isGrounded = rayHit.collider != null && rigid.linearVelocity.y <= 0.1f;

        // 디버그용 선 (Scene 뷰에서 확인 가능)
        Debug.DrawRay(rayOrigin, Vector2.down * halfSize, Color.red);
    }

    /*
    void OnGUI()
    {
        //GUI.Label(new Rect(10, 10, 200, 30), "경사: " + slopeDownAngle, debugLabelStyle);
        GUI.Label(new Rect(10, 10, 200, 30), "isGrounded: " + isGrounded, debugLabelStyle);
    }
    */

    float DebugSlopDownAngle;
    void CheckSlope()
    {
        // 아래 방향으로 레이를 쏴서 경사면의 법선(Normal)을 구함
        float halfSize = capCollider.size.y / 2.0f;
        Vector2 rayOrigin = new Vector2(rigid.position.x, rigid.position.y - halfSize);
        float slopeCheckDistance = halfSize;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, slopeCheckDistance, platformLayer);

        if (hit)
        {
            // 경사면과 수직인 벡터(Normal)를 통해 경사면과 평행한 벡터를 계산
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            DebugSlopDownAngle = slopeDownAngle;
            if (slopeDownAngle != 0)
            {
                isOnSlope = true;
            }
            else
            {
                isOnSlope = false;
            }

            // 디버그용: 경사면 방향 선 그리기
            Debug.DrawRay(hit.point, slopeNormalPerp * 0.5f, Color.green);
        }
    }

    void ApplyMovement()
    {
        if (isGrounded && !isOnSlope) // 평지일 때
        {
            rigid.linearVelocity = new Vector2(inputX * moveSpeed, rigid.linearVelocity.y);
        }
        else if (isGrounded && isOnSlope) // 경사면일 때
        {
            // 경사면의 방향(slopeNormalPerp)을 따라 속도를 재계산
            // inputX가 없을 때(정지 시) 중력을 무시하도록 속도를 0으로 고정하거나 gravityScale 조정
            if (inputX == 0)
            {
                rigid.linearVelocity = Vector2.zero;
                // 정지 시 미끄러짐 방지를 위해 중력을 잠시 끄거나 아주 낮춤
                rigid.gravityScale = 0;
            }
            else
            {
                rigid.gravityScale = 3f; // 원래 중력 수치 (기본값이 3이라면)
                rigid.linearVelocity = new Vector2(moveSpeed * slopeNormalPerp.x * -inputX, moveSpeed * slopeNormalPerp.y * -inputX);
            }
        }
        else // 공중일 때
        {
            rigid.gravityScale = 3f;
            rigid.linearVelocity = new Vector2(inputX * moveSpeed, rigid.linearVelocity.y);
        }
    }


    void Jump()
    {
        rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        PlaySound("JUMP");
    }

    // --- 이벤트 처리 ---
    private void OnCollisionEnter2D(Collision2D collision)
    {      
        Vector3 objectPos = collision.transform.position; // 충돌한 오브젝트의 중심 좌표
        Vector3 hitPoint = collision.contacts[0].point; // 실제 충돌 지점 (플레이어 발이 닿은 위치)

        if (collision.gameObject.CompareTag("MapObjectRoot"))
        {
            // 셀 좌표 변환
            Tilemap tilemap = GameReferences.Instance.GetMapObjectTilemap();
            Vector3Int cellPos = tilemap.WorldToCell(hitPoint);
            TileBase tile = tilemap.GetTile(cellPos);
            
            string tileName = tile != null ? tile.name : "";
            MapObjectType type = MapObjectTableManager.Instance.GetType(tileName);
            if(type == MapObjectType.Damage)
                OnDamaged(collision.transform.position);
        }
        else if(collision.gameObject.CompareTag("Enemy"))
        { 
            // 밟기 판정: 플레이어가 위에서 떨어지는 중이며 적보다 높은 위치일 때
            if (rigid.linearVelocity.y < 0 && transform.position.y > objectPos.y + 0.5f)
            {
                OnAttack(collision.transform);
            }
            else
            {
                OnDamaged(collision.transform.position);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
            HandleItemCollection(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Finish"))
        {
            gameManager.NextStage();
            PlaySound("FINISH");
        }
    }

    void OnAttack(Transform enemy)
    {
        // 적 밟았을 때 반동
        rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, 0); // 기존 수직 속도 초기화
        rigid.AddForce(Vector2.up * 10f, ForceMode2D.Impulse);

        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        if (enemyMove != null) enemyMove.OnDamaged();

        PlaySound("ATTACK");
    }

    void OnDamaged(Vector2 targetPos)
    {
        gameManager.DownPlayerLives();

        // 무적 레이어 변경
        gameObject.layer = LayerMask.NameToLayer("PlayerDamaged");
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // 넉백 효과
        int direction = transform.position.x > targetPos.x ? 1 : -1;
        rigid.AddForce(new Vector2(direction, 1.5f) * 8f, ForceMode2D.Impulse);

        anim.SetTrigger("doDamaged");
        PlaySound("DAMAGED");

        Invoke("OffDamaged", 2f);
    }

    void OffDamaged()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
        spriteRenderer.color = Color.white;
    }

    void HandleItemCollection(GameObject item)
    {
        if (item.name.Contains("Bronze")) gameManager.stagePoint += 50;
        else if (item.name.Contains("Silver")) gameManager.stagePoint += 100;
        else if (item.name.Contains("Gold")) gameManager.stagePoint += 300;

        item.SetActive(false);
        PlaySound("ITEM");
    }

    public void OnDie()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        spriteRenderer.flipY = true;
        capCollider.enabled = false;
        rigid.AddForce(Vector2.up * 5f, ForceMode2D.Impulse);
        PlaySound("DIE");
    }

    void PlaySound(string action)
    {
        switch (action)
        {
            case "JUMP": audioSource.PlayOneShot(audioJump); break;
            case "ATTACK": audioSource.PlayOneShot(audioAttack); break;
            case "DAMAGED": audioSource.PlayOneShot(audioDamaged); break;
            case "ITEM": audioSource.PlayOneShot(audioItem); break;
            case "DIE": audioSource.PlayOneShot(audioDie); break;
            case "FINISH": audioSource.PlayOneShot(audioFinish); break;
        }
    }

    public void VelocityZero()
    {
        rigid.linearVelocity = Vector2.zero;
    }
}