using UnityEngine;

[System.Serializable]
public class SoundBank
{
    public AudioClip clip;
    [Range(0f, 2f)]
    public float volume = 1f;
}

public class SoundFunctions : MonoBehaviour
{
    [SerializeField] SoundBank[] soundBank;

    public void playSound(int soundIndex)
    {
        GameManager.Instance.sfxSource.PlayOneShot(soundBank[soundIndex].clip, soundBank[soundIndex].volume);
    }
}
