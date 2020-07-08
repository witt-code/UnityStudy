using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    
    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update() // 1초에 60회. 단발적인 키 입력.
    {
        //Jump
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
        }
        //Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
                rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }
        //Changing Sprite Direction
        if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }
        //Animation
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
        { anim.SetBool("isWalking", false); }
        else
        { anim.SetBool("isWalking", true); }
    }
    // Update is called once per frame
    void FixedUpdate() // 1초에 50회. 10프레임 손해 봄. 지속적인 키 입력
    {
        //Move Speed
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h * 3, ForceMode2D.Impulse);

        //Max Speed
        if (rigid.velocity.x > maxSpeed)
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1)) // left Max Speed. 음수니까 -1 곱해줌.
            rigid.velocity = new Vector2(maxSpeed*(-1), rigid.velocity.y);

        //Landing Platform
        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("platform"));
            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                {
                    anim.SetBool("isJumping", false);
                }
            }
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
    if(collision.gameObject.tag == "Enemy")
        {
            OnDamaged(collision.transform.position);
        }    
    }

    void OnDamaged(Vector2 targetPos) 
    {
        //Change Layer (Immortality activate)
        gameObject.layer = 11;

        //View Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //Reaction Force
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);

        //Animation
        anim.SetTrigger("doDamaged");
        Invoke("OffDamaged", 3);
    }

    void OffDamaged()
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }
}
