using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuPanel;
    
    [Header("Settings")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
    
    [Header("Audio")]
    [SerializeField] private AudioClip pauseSound;
    [SerializeField] private AudioClip buttonClickSound;
    
    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        isPaused = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        if (isPaused) return;

        isPaused = true;
        
        if (pauseMenuPanel) pauseMenuPanel.SetActive(true);
        
        Time.timeScale = 0f;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        if (pauseSound != null && AudioManager.instance != null)
        {
            AudioManager.instance.StopMusic();
            AudioManager.instance.PlayMusic(pauseSound);
        }
        
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;
        
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        
        Time.timeScale = 1f;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if (AudioManager.instance != null)
        {
            AudioManager.instance.StopMusic();
            AudioManager.instance.PlayLevelMusic();
        }
        
    }

    public void RestartLevel()
    {
        PlayButtonSound();
        
        Time.timeScale = 1f;
        isPaused = false;
        
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);

        if (AudioManager.instance != null)
        {
            AudioManager.instance.StopMusic();
            AudioManager.instance.PlayLevelMusic();
        }
    }

    public void QuitGame()
    {
        PlayButtonSound();
        
        Time.timeScale = 1f;
        
        
        Application.Quit();
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    private void PlayButtonSound()
    {
        if (buttonClickSound != null && AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(buttonClickSound);
        }
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}