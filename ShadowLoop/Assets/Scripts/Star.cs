using UnityEngine;

public class Star : MonoBehaviour
{
    Animator animator;
    [SerializeField] float bashDistance;
    [SerializeField] float distancePercent; // value changed in animation
    [SerializeField] ParticleSystem bashParticles;
    [SerializeField] GameObject particleParent;
    [SerializeField] int emitCount;
    Vector2 startPos;
    Vector2 recoilPos;

    private void Start()
    {
        animator = GetComponent<Animator>();
        startPos = transform.position;
    }

    public void bashedOn(Vector2 bashDir)
    {
        animator.SetTrigger("bash");
        recoilPos = startPos - bashDir * bashDistance;
        if(bashDir == Vector2.up)
        {
            particleParent.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        }
        else if (bashDir == Vector2.left)
        {
            particleParent.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
        }
        else if (bashDir == Vector2.down)
        {
            particleParent.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
        }
        else
        {
            particleParent.transform.localEulerAngles = new Vector3(0f, 0f, 270f);
        }
            bashParticles.Emit(emitCount);
    }

    private void FixedUpdate()
    {
        transform.position = Vector2.Lerp(startPos, recoilPos, distancePercent);
    }
}
