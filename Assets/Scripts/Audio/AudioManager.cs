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

    private void Start()
    {
        // Load saved volumes or default to 1.0f (full volume)
        float musicVol = PlayerPrefs.GetFloat(musicVolumeParam, 1f);
        float sfxVol = PlayerPrefs.GetFloat(sfxVolumeParam, 1f);

        ApplyVolume(musicVolumeParam, musicVol);
        ApplyVolume(sfxVolumeParam, sfxVol);
    }

    public void SetMusicVolume(float volume)
    {
        ApplyVolume(musicVolumeParam, volume);
        PlayerPrefs.SetFloat(musicVolumeParam, volume);
    }

    public void SetSFXVolume(float volume)
    {
        ApplyVolume(sfxVolumeParam, volume);
        PlayerPrefs.SetFloat(sfxVolumeParam, volume);
    }

    private void ApplyVolume(string param, float volume)
    {
        if (volume <= 0.0001f)
            mixer.SetFloat(param, -80f); // Effectively silent
        else
            mixer.SetFloat(param, Mathf.Log10(volume) * 20f);
    }
}
