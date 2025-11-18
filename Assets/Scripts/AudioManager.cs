using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    
    [Header("Music Clips")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private AudioClip drivingMusic; 
    
    [Header("Volume Controls")]
    [SerializeField] [Range(0f, 1f)] private float musicVolume = 0.5f;
    [SerializeField] [Range(0f, 1f)] private float sfxVolume = 1f;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SetupAudioSources();
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MainMenu":
                if (menuMusic != null)
                    PlayMusic(menuMusic);
                break;
            case "Level1":
                if (gameMusic != null)
                    PlayMusic(gameMusic);
                break;
        }
    }
    
    private void SetupAudioSources()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }
        
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
        
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
    }
    
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void StopSFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.Stop();
        }
    }
    
    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip != null && musicSource != null)
        {
            if (musicSource.clip != musicClip)
            {
                musicSource.Stop();
                musicSource.clip = musicClip;
                musicSource.Play();
            }
        }
    }
    
    public void PlayDrivingMusic()
    {
        if (drivingMusic != null)
        {
            PlayMusic(drivingMusic);
        }
    }
    
    public void PlayLevelMusic()
    {
        if (gameMusic != null)
        {
            PlayMusic(gameMusic);
        }
    }
    
    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }
}