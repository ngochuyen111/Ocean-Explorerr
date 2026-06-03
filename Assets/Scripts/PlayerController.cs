
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Di chuyển")]
    public float moveSpeed = 7f;
    public float dashForce = 14f;
    public float dashCooldown = 1f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;

    private float moveX;
    private float moveY;

    private bool facingRight = true;
    private bool canDash = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // A/D hoặc mũi tên trái/phải
        moveX = Input.GetAxisRaw("Horizontal");

        // W/S hoặc mũi tên lên/xuống
        moveY = Input.GetAxisRaw("Vertical");

        // Dash: Shift trái
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        UpdateFacing();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveX * moveSpeed, moveY * moveSpeed);
    }

    IEnumerator Dash()
    {
        canDash = false;

        float dashDirection = facingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dashDirection * dashForce, rb.linearVelocity.y);

        if (anim != null)
        {
            anim.SetTrigger("Dash");
        }

        yield return new WaitForSeconds(0.2f);
        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    void UpdateFacing()
    {
        if (moveX > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveX < 0 && facingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void UpdateAnimation()
    {
        if (anim == null) return;

        bool isMoving = Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveY) > 0.1f;
        anim.SetBool("Swim", isMoving);
        anim.SetFloat("VerticalSpeed", rb.linearVelocity.y);
    }
}