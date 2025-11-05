using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // SINGLETON
    public static AudioManager instance;
    private AudioSource audioSource;
    [SerializeField] private AudioClip backgroundMusic; // Clip de música de fondo
    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Mantiene el AudioManager entre escenas
        }
        else
        {
            Destroy(gameObject); // Destruye el AudioManager si ya existe uno
        }
    }
    
    // Método para reproducir un clip de audio
    public void PlayAudioClip(AudioClip clip)
    {
        // correr el clip de audio aunque no este en el audioSource
        audioSource.PlayOneShot(clip);
    }
    
    // Método para detener la reproducción de audio
    public void PlaySoundFromLocation(AudioClip clip, Vector3 position)
    {
        AudioSource.PlayClipAtPoint(clip, position);
    }
    
    // Método para reproducir música de fondo
    private void Start()
    {
        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true; // Reproduce en bucle
            audioSource.Play(); // Inicia la reproducción de música de fondo
        }
    }
}
