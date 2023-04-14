using System.Collections;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] GameObject explosion;
    GameObject player;
    PlayerAnimation playerAnimation;
    Animator animator;
    float delayTime;
    bool varSet = false;

    public void setup(GameObject newPlayer, float newDelayTime)
    {
        player = newPlayer;
        playerAnimation = newPlayer.GetComponent<PlayerAnimation>();
        delayTime = newDelayTime;
        animator = GetComponent<Animator>();
        varSet = true;
        setVariables(newPlayer.transform.position, 0, false);
        StartCoroutine(enableShadow());
    }

    private void FixedUpdate()
    {
        if (!varSet) return;
        StartCoroutine(holdPlayerInfo(player.transform.position, playerAnimation.getAnimationIndex(), playerAnimation.getFlipSprite()));
    }

    IEnumerator holdPlayerInfo(Vector2 newPosition, int animationIndex, bool flipSprite)
    {
        yield return new WaitForSeconds(delayTime);
        setVariables(newPosition, animationIndex, flipSprite);
    }

    void setVariables(Vector2 newPosition, int animationIndex, bool flipSprite)
    {
        transform.position = newPosition;
        animator.SetInteger("animationIndex", animationIndex);
        spriteRenderer.flipX = flipSprite;
    }

    IEnumerator enableShadow()
    {
        yield return new WaitForSeconds(delayTime);
        spriteRenderer.enabled = true;
    }

    public void destroy()
    {
        if (spriteRenderer.enabled == true) Instantiate(explosion, transform.position, Quaternion.identity, null);
        Destroy(gameObject);
    }
}
