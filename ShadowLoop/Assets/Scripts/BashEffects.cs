using UnityEngine;

public class BashEffects : MonoBehaviour
{
    [SerializeField] GameObject arrow;
    Animator arrowAnimator;
    TrailRenderer trail;

    private void Start()
    {
        trail = GetComponent<TrailRenderer>();
        arrowAnimator = arrow.GetComponent<Animator>();
        changeArrowEnabled(false);
    }

    public void changeArrowDir(Vector2 dir)
    {
        if(dir == Vector2.up)
        {
            arrowAnimator.SetInteger("dir", 0);
        }

        if (dir == Vector2.left)
        {
            arrowAnimator.SetInteger("dir", 1);
        }

        if (dir == Vector2.down)
        {
            arrowAnimator.SetInteger("dir", 2);
        }

        if (dir == Vector2.right)
        {
            arrowAnimator.SetInteger("dir", 3);
        }
    }

    public void changeArrowEnabled(bool value)
    {
        arrow.gameObject.SetActive(value);
    }

    public void endTrail()
    {
        trail.emitting = false;
    }

    public void startTrail()
    {
        trail.emitting = true;
    }
}
