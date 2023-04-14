using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] GameObject sprite;
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;
    PlayerPhysics playerPhysics;
    bool flipSprite;
    int animationIndex;
    /*animation index represents which animation to play based on a int:
     * 0 = Idle
     * 1 = Run
     * 2 = Jump
     * 3 = Fall
     */

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerPhysics = GetComponent<PlayerPhysics>();
        spriteRenderer = sprite.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if(rb.velocity.x < -0.1f && Input.GetAxisRaw("Horizontal") == -1)
        {
            flipSprite = true;
        }else if(rb.velocity.x > 0.1f && Input.GetAxisRaw("Horizontal") == 1)
        {
            flipSprite = false;
        }

        if(playerPhysics.playerIsBashing)
        {
            animationIndex = 2;
        }else if(!playerPhysics.isGrounded)
        {
            if(rb.velocity.y > 0)
            {
                animationIndex = 2;
            }else
            {
                animationIndex = 3;
            }
        }else if (Input.GetAxisRaw("Horizontal") != 0)
        {
            animationIndex = 1;
        }else
        {
            animationIndex = 0;
        }

        spriteRenderer.flipX = flipSprite;
        animator.SetInteger("animationIndex", animationIndex);
    }

    public int getAnimationIndex()
    {
        return animationIndex;
    }

    public bool getFlipSprite()
    {
        return flipSprite;
    }
}
