using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capCollider;
    bool isPlayMode = false;
    
    public int nextMove;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capCollider = GetComponent<CapsuleCollider2D>();

        Invoke("Think", 5);
    }

    void Start()
    {
        EventManager.Instance.OnChangePlayMode += HandleChangePlayMode;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnChangePlayMode -= HandleChangePlayMode;
    }

    void FixedUpdate()
    {
        if (rigid.simulated == false)
            return;

        //Move
        rigid.linearVelocity = new Vector2(nextMove, rigid.linearVelocity.y);

        //Platform Check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.2f, rigid.position.y);

        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null)
        {
            Turn();
        }
    }

    void Think()
    {
        //Set Next Active
        if (isPlayMode)
            nextMove = Random.Range(-1, 2);
        else
            nextMove = 0;

            //Sprite Animation
            anim.SetInteger("walkSpeed", nextMove);

        //Flip Sprite
        if (nextMove != 0)
            spriteRenderer.flipX = nextMove == 1;

        //Recursive
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);
    }

    void Turn()
    {
        //Debug.Log("경고! 이 앞 낭떨어지!!.!.");
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;

        CancelInvoke();
        Invoke("Think", 2);
    }

    public void OnDamaged()
    {
        //Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //Sprite Flip Y
        spriteRenderer.flipY = true;
        //Collider Disable
        capCollider.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //Destroy
        Invoke("DeActive", 5);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }

    void HandleChangePlayMode(bool mode)
    {
        isPlayMode = mode;
    }
}
