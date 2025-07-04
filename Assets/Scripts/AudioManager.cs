using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioMixer mixer;

    [Header("Mixer Parameter Names")]
    public string musicVolumeParam = "MusicVolume";
    public string sfxVolumeParam = "SFXVolume";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureInstanceExists()
    {
        if (Instance == null)
        {
            GameObject audioManagerObj = new GameObject("AudioManager (Auto)");
            audioManagerObj.AddComponent<AudioManager>();
        }
    }

    public void SetMusicVolume(float volume)
    {
        mixer.SetFloat(musicVolumeParam, Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(musicVolumeParam, volume);
    }

    public void SetSFXVolume(float volume)
    {
        mixer.SetFloat(sfxVolumeParam, Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(sfxVolumeParam, volume);
    }
}
