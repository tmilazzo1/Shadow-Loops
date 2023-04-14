using UnityEngine;

public class TimeLoop : MonoBehaviour
{
    [Header("Unity Setup")]

    [SerializeField] GameObject particleExplosionPrefab;
    [SerializeField] GameObject shadowPrefab;
    [SerializeField] GameObject linePrefab;
    [SerializeField] int sfxLoopIndex;
    SoundFunctions soundFunctions;
    GameObject currentShadow;
    Vector2 shadowPos;

    [Header("Attributes")]

    [SerializeField] float shadowDelayTime;
    [SerializeField] float cooldownTime;
    [SerializeField] float slowStrength;
    [SerializeField] float slowDuration;
    [SerializeField] float lineDuration;
    bool loopOnCooldown = true;
    float counter;
    bool loopInputed;
    bool bashLoopInputed;
    bool holdInput;
    bool buttonDown;

    private void Start()
    {
        soundFunctions = GetComponent<SoundFunctions>();
        counter = cooldownTime;
    }

    private void FixedUpdate()
    {
        if (loopOnCooldown)
        {
            counter -= Time.deltaTime;
        }
    }

    private void Update()
    {
        inputManager();

        if(counter <= shadowDelayTime + 0.5f && !currentShadow)
        {
            spawnShadow();
        }

        if(counter <= 0 && loopOnCooldown)
        {
            loopOnCooldown = false;
        }

        if (holdInput) return;

        if((loopInputed || bashLoopInputed) && !loopOnCooldown)
        {
            loopInputed = false;
            bashLoopInputed = false;
            loopOnCooldown = true;
            shadowPos = currentShadow.transform.position;
            timeLoop();
        }
    }

    void inputManager()
    {
        if (buttonDown) loopInputed = false;

        if (Input.GetAxisRaw("Fire2") == 1 && !buttonDown)
        {
            buttonDown = true;
            loopInputed = true;
            if (holdInput) bashLoopInputed = true;
        }else if(Input.GetAxisRaw("Fire2") != 1)
        {
            buttonDown = false;
        }
    }

    public void bashInProgress(bool bashing)
    {
        holdInput = bashing;
    }

    void timeLoop()
    {
        GameManager.Instance.slowTime(slowStrength, slowDuration);
        Instantiate(particleExplosionPrefab, shadowPos, Quaternion.identity, null);
        GameObject newLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity, null);
        newLine.GetComponent<LineFade>().setup(transform.position, shadowPos, lineDuration);
        soundFunctions.playSound(sfxLoopIndex);

        counter = cooldownTime;
        transform.position = shadowPos;
        Destroy(currentShadow);
    }

    void spawnShadow()
    {
        if (currentShadow) return;
        GameObject newShadow = Instantiate(shadowPrefab, transform.position, Quaternion.identity, null);
        newShadow.GetComponent<Shadow>().setup(gameObject, shadowDelayTime);
        currentShadow = newShadow;
    }
}
