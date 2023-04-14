using UnityEngine;

public class StartMenu : MonoBehaviour
{
    bool gameStarted;

    private void Update()
    {
        if(Input.GetAxisRaw("Submit") == 1 && !gameStarted)
        {
            gameStarted = true;
            GameManager.Instance.levelComplete();
        }
    }
}
