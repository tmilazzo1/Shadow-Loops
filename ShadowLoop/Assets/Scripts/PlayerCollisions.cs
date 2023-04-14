using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] string deathBoxTag;
    [SerializeField] int portalLayer;
    [SerializeField] float radiusMultiplier;
    [SerializeField] int sfxPortalIndex;
    [SerializeField] int sfxDeathIndex;
    SoundFunctions soundFunctions;
    float distanceFromPortal;
    bool portalCollision;
    float angle;
    Vector2 portalPos;

    private void Start()
    {
        soundFunctions = GetComponent<SoundFunctions>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == portalLayer)
        {
            onPortalCollision(col.gameObject);
        }

        if(col.gameObject.tag == deathBoxTag)
        {
            onDeath();
        }
    }

    void onPortalCollision(GameObject portal)
    {
        GetComponent<PlayerPhysics>().enabled = false;
        GetComponent<TimeLoop>().enabled = false;
        GetComponent<BashEffects>().enabled = false;
        GetComponent<PlayerAnimation>().enabled = false;
        Destroy(GetComponent<Rigidbody2D>());
        GameManager.Instance.changeTimeScale(1, 0);
        if(GameObject.FindGameObjectWithTag("shadow")) GameObject.FindGameObjectWithTag("shadow").GetComponent<Shadow>().destroy();
        soundFunctions.playSound(sfxPortalIndex);
        
        portalPos = portal.transform.position;
        distanceFromPortal = Mathf.Sqrt((portalPos.x - transform.position.x) * (portalPos.x - transform.position.x) + (portalPos.y - transform.position.y) * (portalPos.y - transform.position.y));
        angle = Mathf.Atan((transform.position.y - portalPos.y) / (transform.position.x - portalPos.x));
        if (transform.position.x < portalPos.x) angle += Mathf.PI;
        GetComponent<Animator>().SetInteger("animationIndex", 4);
        portalCollision = true;
    }

    public void onDeath()
    {
        soundFunctions.playSound(sfxDeathIndex);
        Instantiate(explosionPrefab, transform.position, Quaternion.identity, null);
        if (GameObject.FindGameObjectWithTag("shadow")) GameObject.FindGameObjectWithTag("shadow").GetComponent<Shadow>().destroy();
        GameManager.Instance.changeTimeScale(1, 0);
        GameManager.Instance.reloadLevel();
        Destroy(gameObject);
    }

    private void Update()
    {
        if (!portalCollision) return;

        transform.position = new Vector2(portalPos.x + radiusMultiplier * distanceFromPortal * Mathf.Cos(angle), portalPos.y + radiusMultiplier * distanceFromPortal * Mathf.Sin(angle));
    }

    public float getRadiusMultiplier()
    {
        return radiusMultiplier;
    }

    public void endingAnimationFinished()
    {
        Destroy(gameObject);
        GameManager.Instance.levelComplete();
    }
}
