using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public enum SoundType
    {
        Walk,
        Jump,
        PowerUp,
        Attack,
        TakeDamage,
        PickUp
        // Add more sound types as needed
    }

    [System.Serializable]
    private class SoundEntry
    {
        [Header("Sound Type")]
        public SoundType type;

        [Header("Playback Settings")]
        public AudioSource audioSource;
        [Range(0f, 1f)] public float volume = 1f;
        [Min(0f)] public float delayBetweenSounds = 0f;

        [Header("SFX")]
        public AudioClip[] audioClips;
    }

    [SerializeField] private List<SoundEntry> soundEffects;

    private Dictionary<SoundType, SoundEntry> soundMap;
    private Dictionary<SoundType, bool> canPlay = new();

    void Awake()
    {
        soundMap = new Dictionary<SoundType, SoundEntry>();
        foreach (var effect in soundEffects)
        {
            if (effect.audioClips != null && effect.audioClips.Length > 0 && effect.audioSource != null)
            {
                soundMap[effect.type] = effect;
                canPlay[effect.type] = true;
            }
        }
    }

    void PlaySound(SoundType type)
    {
        if (!soundMap.TryGetValue(type, out var effect)) return;
        if (!canPlay.TryGetValue(type, out bool canBePlayed) || !canBePlayed) return;

        var audioClip = effect.audioClips[Random.Range(0, effect.audioClips.Length)];
        effect.audioSource.PlayOneShot(audioClip, effect.volume);

        if (effect.delayBetweenSounds > 0f)
        {
            StartCoroutine(SoundCooldown(type, effect.delayBetweenSounds));
        }
    }

    IEnumerator SoundCooldown(SoundType type, float delay)
    {
        canPlay[type] = false;
        yield return new WaitForSeconds(delay);
        canPlay[type] = true;
    }

    #region Helpers
    // These make calling the sound effects from other scripts easier
    public void PlayWalk()      => PlaySound(SoundType.Walk);
    public void PlayJump()      => PlaySound(SoundType.Jump);
    public void PlayPowerUp()   => PlaySound(SoundType.PowerUp);
    public void PlayAttack()    => PlaySound(SoundType.Attack);
    public void PlayDamage()    => PlaySound(SoundType.TakeDamage);
    public void PlayPickUp()    => PlaySound(SoundType.PickUp);

    #endregion
}