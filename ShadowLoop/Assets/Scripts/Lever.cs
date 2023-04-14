using UnityEngine;
using UnityEngine.Events;

public class Lever : MonoBehaviour
{
    [SerializeField] UnityEvent onFlipEvents;
    SoundFunctions soundFunctions;
    Animator animator;
    bool on;

    private void Start()
    {
        soundFunctions = GetComponent<SoundFunctions>();
        animator = GetComponent<Animator>();
    }

    public void highlight(bool value)
    {
        animator.SetBool("highlight", value);
    }

    public void flip()
    {
        on = !on;
        animator.SetBool("on", on);
        onFlipEvents.Invoke();
        soundFunctions.playSound(0);
    }

    public void changeState()
    {
        on = !on;
        animator.SetBool("on", on);
    }
}
