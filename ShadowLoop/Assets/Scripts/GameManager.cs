using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    [Header("Unity Setup")]

    [SerializeField] ScreenTransition screenTransition;

    bool pauseButtonDown;
    bool cantPause = true;
    bool onCredits = false;
    [HideInInspector] public bool gameIsPaused { get; private set; }
    [SerializeField] GameObject pauseMenu;

    [Header("Audio")]

    public AudioSource sfxSource;
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioMixer mixer;
    [SerializeField] float maxVolLoss;
    [SerializeField] float maxPitchLoss;
    [SerializeField] float maxFreqLoss;
    float startVol;
    float startPitch;
    float startFreq;

    //slowTime variables

    float oldTimeScale;
    float newTimeScale;
    float lerpDuration;
    bool lerpTimeScale = false;
    float lerpPercent;
    float lerpCounter;

    private void Start()
    {
        startVol = musicSource.volume;
        startPitch = musicSource.pitch;
        mixer.GetFloat("freq", out startFreq);
    }

    public void credits()
    {
        cantPause = true;
        onCredits = true;
        pauseGame(false);
    }

    public void levelComplete()
    {
        cantPause = true;
        pauseGame(false);
        screenTransition.fadeOut(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void reloadLevel()
    {
        cantPause = true;
        pauseGame(false);
        screenTransition.fadeOut(SceneManager.GetActiveScene().buildIndex);
    }

    public void levelLoaded()
    {
        pauseGame(false);
        if (onCredits) return;
        cantPause = false;
    }

    private void Update()
    {
        if(Input.GetAxisRaw("Cancel") == 1 && !pauseButtonDown)
        {
            pauseButtonDown = true;
            if (onCredits) Application.Quit();
            if (!cantPause)
            {
                gameIsPaused = !gameIsPaused;
                pauseGame(gameIsPaused);
            }
        }else if(Input.GetAxisRaw("Cancel") != 1)
        {
            pauseButtonDown = false;
        }
    }

    public void pauseGame(bool pause)
    {
        pauseMenu.SetActive(pause);
        gameIsPaused = pause;
        if(pause)
        {
            changeTimeScale(0, 0);
        }else
        {
            changeTimeScale(1, 0);
        }
    }

    public void changeTimeScale(float strength, float lerpDur)
    {
        if(strength > 1 || strength < 0 || lerpDur < 0)
        {
            Debug.Log("slow strength error");
            return;
        }

        if(lerpDur == 0)
        {
            changeMusicPitch(1 - strength);
            Time.timeScale = strength;
            lerpTimeScale = false;
        }else
        {
            oldTimeScale = Time.timeScale;
            newTimeScale = strength;
            lerpDuration = lerpDur;
            lerpTimeScale = true;
        }
    }

    public void slowTime(float strength, float dur)
    {
        if (strength > 1 || strength <= 0 || dur <= 0)
        {
            Debug.Log("slow strength error");
            return;
        }

        oldTimeScale = strength;
        lerpCounter = 0;
        lerpPercent = 0;
        newTimeScale = 1;
        lerpDuration = dur;
        lerpTimeScale = true;
    }

    private void FixedUpdate()
    {
        if(lerpTimeScale) //lerp time scale back to one
        {
            lerpCounter += Time.deltaTime;
            lerpPercent = lerpCounter / lerpDuration;
            float newScale = Mathf.Lerp(oldTimeScale, newTimeScale, lerpPercent);
            Time.timeScale = newScale;
            changeMusicPitch(1 - newScale);
        }
        
        if(lerpPercent > 1) //resets variables
        {
            lerpTimeScale = false;
            lerpPercent = 0;
            lerpCounter = 0;
        }
    }

    void changeMusicPitch(float percentage)
    {
        musicSource.volume = startVol - percentage * maxVolLoss;
        musicSource.pitch = startPitch - percentage * maxPitchLoss;
        mixer.SetFloat("freq", startFreq - percentage * maxFreqLoss);
    }
}
