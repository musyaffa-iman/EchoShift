using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("Music")]
    [SerializeField] public AudioClip mainMenuMusic;
    [SerializeField] public AudioClip backgroundMusic;
    [SerializeField] public AudioClip bossMusic;

    [Header("Sound Effects")]
    [SerializeField] public AudioClip buttonClick;
    [SerializeField] public AudioClip playerAttack;
    [SerializeField] public AudioClip playerHit;
    [SerializeField] public AudioClip enemyAttack;
    [SerializeField] public AudioClip enemyHit;
    [SerializeField] public AudioClip objectBreak;
    [SerializeField] public AudioClip coinPickup;
    [SerializeField] public AudioClip potionPickup;
    [SerializeField] public AudioClip portal;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlayMusic(AudioClip clip)
    {
        PlayMusic(clip, 1f);
    }
    
    public void PlayMusic(AudioClip clip, float volume)
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

}
