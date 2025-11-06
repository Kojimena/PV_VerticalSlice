using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // SINGLETON
    public static AudioManager instance;
    
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;    // Para música de fondo
    [SerializeField] private AudioSource sfxSource;      // Para efectos de sonido
    
    [Header("Music")]
    [SerializeField] private AudioClip backgroundMusic;
    
    [Header("Volume Controls")]
    [SerializeField] [Range(0f, 1f)] private float musicVolume = 0.5f;
    [SerializeField] [Range(0f, 1f)] private float sfxVolume = 1f;
    
    private void Awake()
    {
        // Singleton 
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SetupAudioSources();
        }
        else
        {
            Destroy(gameObject);
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
    
    private void Start()
    {
        if (backgroundMusic != null)
        {
            PlayMusic(backgroundMusic);
        }
    }
    
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
    
    public void PlaySoundFromLocation(AudioClip clip, Vector3 position)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position, sfxVolume);
        }
    }
    
    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip != null && musicSource != null)
        {
            musicSource.Stop();
            musicSource.clip = musicClip;
            musicSource.Play();
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