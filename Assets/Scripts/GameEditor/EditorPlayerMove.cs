using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class EditorPlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;

    [Header("--- Movement Settings ---")]
    public float moveSpeed = 5f;      // 이동 속도

    // 내부 변수
    float inputX;
    float inputY;

    private GUIStyle debugLabelStyle;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        rigid.gravityScale = 0;

        debugLabelStyle = new GUIStyle();
        debugLabelStyle.fontSize = 30;
        debugLabelStyle.fontStyle = FontStyle.Bold;
        debugLabelStyle.normal.textColor = Color.red;
    }

    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        if (inputX != 0)
        {
            spriteRenderer.flipX = inputX == -1;
        }

        if (inputX != 0 || inputY != 0)
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);
    }

    void FixedUpdate()
    {
        ApplyMovement(); // 이동 로직 분리
    }

    void OnGUI()
    {
    }

    void ApplyMovement()
    {
        rigid.linearVelocity = new Vector2(inputX * moveSpeed, inputY * moveSpeed);
    }
}