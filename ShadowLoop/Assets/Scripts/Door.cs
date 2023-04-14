using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] bool doorEnabled;
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("enabled", doorEnabled);
    }

    public void changeEnabled()
    {
        doorEnabled = !doorEnabled;
        animator.SetBool("enabled", doorEnabled);
    }
}
