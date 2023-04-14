using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreenTransition : MonoBehaviour
{
    int levelIndex = 0;
    Animator animator;
    [SerializeField] Text levelText;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void fadeOut(int index)
    {
        levelIndex = index;
        animator.SetTrigger("fadeOut");
    }

    public void loadNextLevel()
    {
        SceneManager.LoadScene(levelIndex);
    }

    public void fadeIn()
    {
        Time.timeScale = 0;
        levelText.text = SceneManager.GetSceneByBuildIndex(levelIndex).name;
    }

    public void fadeInFinished()
    {
        GameManager.Instance.levelLoaded();
    }

    public void changeTimeScale(float strength)
    {
        GameManager.Instance.changeTimeScale(strength, 0);
    }
}
