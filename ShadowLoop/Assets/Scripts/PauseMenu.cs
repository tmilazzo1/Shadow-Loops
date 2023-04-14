using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    Animator animator;
    int index;
    bool verticalButtonDown;
    bool confirmButtonDown;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        animator.SetInteger("index", index);
    }

    private void Update()
    {
        if(Input.GetAxisRaw("Vertical") != 0 && !verticalButtonDown)
        {
            verticalButtonDown = true;
            if(Input.GetAxisRaw("Vertical") == 1)
            {
                if (index != 0) index--;
            }else
            {
                if (index != 2) index++;
            }
            animator.SetInteger("index", index);
        }
        else if(Input.GetAxisRaw("Vertical") == 0)
        {
            verticalButtonDown = false;
        }

        if(Input.GetAxisRaw("Submit") == 1 && !confirmButtonDown)
        {
            confirmButtonDown = true;
            if(index == 0)
            {
                continuePressed();
            }else if(index == 1)
            {
                restartPressed();
            }else
            {
                quitPressed();
            }
        }else if(Input.GetAxisRaw("Submit") != 1)
        {
            confirmButtonDown = false;
        }
    }

    void continuePressed()
    {
        GameManager.Instance.pauseGame(false);
    }

    void restartPressed()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCollisions>().onDeath();
    }

    void quitPressed()
    {
        Application.Quit();
    }
}
