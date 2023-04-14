using UnityEngine;
using System.Collections;

public class PlayerPhysics : MonoBehaviour
{
    [Header("Unity Setup")]

    [SerializeField] Transform feetPos;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] FindStar findStar;
    SoundFunctions soundFunctions;
    TimeLoop timeLoop;
    BashEffects bashEffects;
    Rigidbody2D rb;
    static float baseGrav;

    [Header("Horizontal")]

    [SerializeField] float maxSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float xAccelerationOnBash;
    [SerializeField] float gravityOnBash;
    [SerializeField] float flyingRecoveryDuration;
    float gravityCounter;
    bool newHorizontalInput;
    float currentAcceleration;
    float targetVelocity;
    float currentVelocity;
    int xInput;
    int prevXInput;
    int accelerationDir;
    bool flying;
    float flyingCounter;
    float flyingMultiplier;

    [Header("Vertical")]

    [SerializeField] float jumpPower;
    [SerializeField] float saveJumpTime;
    [SerializeField] float coyoteTime;
    [SerializeField] float yAccelerationOnBash;
    float coyoteCounter;
    float jumpCounter;
    bool canLand;
    bool jumpButtonDown;
    bool jumpIsInputed;
    [HideInInspector] public bool isGrounded { get; private set; }

    [Header("Bash")]

    [SerializeField] Vector2 bashPower;
    [SerializeField] float bashDuration;
    [SerializeField] float radiusFromStar;
    [SerializeField] float moveTime;
    [SerializeField] float bashCooldown;
    float bashCooldownCounter;
    GameObject starObj;
    Vector2 bashPosition;
    Vector2 initialPosition;
    float moveCounter;
    float bashCounter;
    Vector2 bashDir;
    bool bashButtonDown;
    [HideInInspector] public bool playerIsBashing { get; private set; }

    [Header("Bash Slow Time")]

    [SerializeField] float timeSlow;
    [SerializeField] float timeLerpIn;
    [SerializeField] float timeLerpOut;

    [Header("Sound")]

    [SerializeField] int landIndex;
    [SerializeField] int jumpIndex;
    [SerializeField] int bashIndex;

    private void Start()
    {
        soundFunctions = GetComponent<SoundFunctions>();
        rb = GetComponent<Rigidbody2D>();
        baseGrav = rb.gravityScale;
        currentAcceleration = acceleration;
        bashDir = Vector2.up;
        bashEffects = GetComponent<BashEffects>();
        timeLoop = GetComponent<TimeLoop>();
    }

    private void Update()
    {
        if(playerIsBashing) bashOnDown();
    }

    private void FixedUpdate()
    {
        inputManager();
        if (bashCooldownCounter >= 0) bashCooldownCounter -= Time.deltaTime;
        if (jumpCounter >= 0) jumpCounter -= Time.deltaTime;
        if (coyoteCounter >= 0) coyoteCounter -= Time.deltaTime;

        if (playerIsBashing)
        {
            if (moveCounter <= moveTime) moveCounter += Time.unscaledDeltaTime;
            bashCounter -= Time.unscaledDeltaTime;
            if (bashCounter <= 0)
            {
                playerIsBashing = false;
                bashOnRelease();
            }
            return;
        }

        horizontalMovement();
        checkGrounded();
        if (jumpIsInputed) jump();
    }

    void inputManager()
    {
        xInput = (int)Input.GetAxisRaw("Horizontal");

        if (Input.GetAxisRaw("Jump") == 1 && !jumpButtonDown)
        {
            jumpButtonDown = true;
            jumpIsInputed = true;
            jumpCounter = saveJumpTime;
        }
        else if (Input.GetAxisRaw("Jump") == 0)
        {
            jumpButtonDown = false;
        }

        if (jumpCounter < 0) jumpIsInputed = false;

        if (Input.GetAxisRaw("Fire1") == 1 && !bashButtonDown && bashCooldownCounter <= 0)
        {
            bashButtonDown = true;
            bashOnPress();
        }
        else if (Input.GetAxisRaw("Fire1") == 0)
        {
            bashButtonDown = false;
            if (playerIsBashing) bashOnRelease();
        }
    }

    void horizontalMovement()
    {
        if(rb.gravityScale != baseGrav)
        {
            gravityCounter += Time.deltaTime;
            rb.gravityScale = Mathf.Lerp(gravityOnBash, baseGrav, gravityCounter / flyingRecoveryDuration);
        }else
        {
            gravityCounter = 0;
        }

        if (xInput != prevXInput) newHorizontalInput = true;

        if(newHorizontalInput)
        {
            newHorizontalInput = false;
            prevXInput = xInput;
            targetVelocity = xInput * maxSpeed;
            currentVelocity = rb.velocity.x;
            if(targetVelocity > currentVelocity)
            {
                accelerationDir = 1;
            }else
            {
                accelerationDir = -1;
            }
        }

        if (flying && flyingCounter <= flyingRecoveryDuration)
        {
            flyingCounter += Time.deltaTime;
            flyingMultiplier = flyingCounter / flyingRecoveryDuration;
        }else
        {
            flyingMultiplier = 1;
        }

        if (accelerationDir == 1)
        {
            if(currentVelocity < targetVelocity)
            {
                currentVelocity += Time.deltaTime * accelerationDir * currentAcceleration * maxSpeed * flyingMultiplier;
            }else
            {
                currentVelocity = targetVelocity;
            }
        }else if(accelerationDir == -1)
        {
            if (currentVelocity > targetVelocity)
            {
                currentVelocity += Time.deltaTime * accelerationDir * currentAcceleration * maxSpeed * flyingMultiplier;
            }else
            {
                currentVelocity = targetVelocity;
            }
        }

        rb.velocity = new Vector2(currentVelocity, rb.velocity.y);
    }

    void jump()
    {
        if (isGrounded || coyoteCounter > 0)
        {
            canLand = true;
            coyoteCounter = 0;
            isGrounded = false;
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            soundFunctions.playSound(jumpIndex);
        }
    }

    void checkGrounded()
    {
        bool value = true;

        if (!Physics2D.OverlapCircle(feetPos.position, 0.1f, whatIsGround))
        {
            value = false;
        }

        if (rb.velocity.y > 0.1f && rb.velocity.y < -0.1f)
        {
            value = false;
        }

        if (!isGrounded && value && canLand)
        {
            canLand = false;
            soundFunctions.playSound(landIndex);
            StartCoroutine(changeCanLand());
        }
        if (isGrounded && !value) coyoteCounter = coyoteTime;

        isGrounded = value;
        if (value && flying)
        {
            flying = false;
            currentAcceleration = acceleration;
            newHorizontalInput = true;
            bashEffects.endTrail();
        }
    }

    IEnumerator changeCanLand()
    {
        yield return new WaitForSeconds(0.1f);
        canLand = true;
    }





    void bashOnPress()
    {
        starObj = findStar.getClosestStar();
        if(starObj != null)
        {
            playerIsBashing = true;
            GameManager.Instance.changeTimeScale(timeSlow, timeLerpIn);

            float angle = getBashAngle();
            bashPosition = new Vector2(getBashPosX(angle), getBashPosY(angle));
            initialPosition = transform.position;
            moveCounter = 0;
            bashCounter = bashDuration;
            bashEffects.changeArrowEnabled(true);
            bashEffects.changeArrowDir(bashDir);
            timeLoop.bashInProgress(true);
        }
    }

    void bashOnDown()
    {
        rb.velocity = Vector2.zero;
        transform.position = Vector2.Lerp(initialPosition, bashPosition, moveCounter / moveTime);

        if(Input.GetKey(KeyCode.W))
        {
            bashDir = Vector2.up;
            bashEffects.changeArrowDir(bashDir);
            return;
        }

        if (Input.GetKey(KeyCode.A))
        {
            bashDir = Vector2.left;
            bashEffects.changeArrowDir(bashDir);
            return;
        }

        if (Input.GetKey(KeyCode.S))
        {
            bashDir = Vector2.down;
            bashEffects.changeArrowDir(bashDir);
            return;
        }

        if (Input.GetKey(KeyCode.D))
        {
            bashDir = Vector2.right;
            bashEffects.changeArrowDir(bashDir);
            return;
        }
    }

    void bashOnRelease()
    {
        starObj.GetComponentInParent<Star>().bashedOn(bashDir);
        if(bashDir.x != 0)
        {
            rb.velocity = bashDir * bashPower.x;
            rb.gravityScale = gravityOnBash;
            currentAcceleration = xAccelerationOnBash;
        }else
        {
            rb.velocity = bashDir * bashPower.y;
            currentAcceleration = yAccelerationOnBash;
        }
        currentVelocity = bashDir.x * bashPower.x;
        targetVelocity = bashDir.x * (maxSpeed + (bashPower.x - maxSpeed) / 2);

        bashCooldownCounter = bashCooldown;
        soundFunctions.playSound(bashIndex);
        flying = true;
        bashEffects.changeArrowEnabled(false);
        bashEffects.startTrail();
        playerIsBashing = false;
        GameManager.Instance.changeTimeScale(1, timeLerpOut);
        timeLoop.bashInProgress(false);
    }





    float getBashAngle()
    {
        float v = (transform.position.y - starObj.transform.position.y) / (transform.position.x - starObj.transform.position.x);
        if(transform.position.x > starObj.transform.position.x)
        {
            return Mathf.Atan(v);
        }else
        {
            return Mathf.Atan(v) + Mathf.PI;
        }
    }

    float getBashPosX(float angle)
    {
        return starObj.transform.position.x + radiusFromStar * Mathf.Cos(angle);
    }

    float getBashPosY(float angle)
    {
        return starObj.transform.position.y + radiusFromStar * Mathf.Sin(angle);
    }
}
