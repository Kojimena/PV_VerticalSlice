using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("GameManager");
                _instance = go.AddComponent<GameManager>();
            }
            return _instance;
        }
    }

    public int currentLevel = 1;
    public int totalLevels = 3;


    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"<color=cyan>[GameManager]</color> Escena cargada: {scene.name}");
        
        Time.timeScale = 1f;
        
        if (scene.buildIndex >= 2 && scene.buildIndex <= 5) 
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void LoadLevel(int levelIndex)
    {
        currentLevel = levelIndex;
        LoadSceneAsync(levelIndex + 1); 
    }

    public void LoadNextLevel()
    {
        if (currentLevel < totalLevels)
        {
            currentLevel++;
            
            ResetLevelProgress();
            
            LoadSceneAsync(currentLevel + 1);
        }
        else
        {
            LoadWinScene();
        }
    }

    private void ResetLevelProgress()
    {
        if (PersistenceManager.Instance == null) return;

        Debug.Log("<color=yellow>[GameManager]</color> Reseteando progreso para nuevo nivel...");

        PersistenceManager.Instance.data.currentLives = PersistenceManager.Instance.data.maxLives;
        Debug.Log($"<color=green>[GameManager]</color> Vidas reseteadas a {PersistenceManager.Instance.data.maxLives}");
            
        PersistenceManager.Instance.data.inventoryItems.Clear();
        Debug.Log("<color=green>[GameManager]</color> Inventario limpiado");
        

        PersistenceManager.Instance.data.collectedItemIds.Clear();
        
        PersistenceManager.Instance.data.isDoorOpen = false;
        
        PersistenceManager.Instance.data.hasCheckpoint = false;
        PersistenceManager.Instance.data.playerPosition = Vector3.zero;

        PersistenceManager.Instance.SaveSessionData(PersistenceManager.Instance.data);
    }

    private void LoadWinScene()
    {
        LoadSceneAsync(6);
    }

    public void LoadMainMenu()
    {
        LoadSceneAsync(0);
    }
    
    public void LoadSceneByName(string sceneName)
    {
        StartCoroutine(LoadSceneByNameCoroutine(sceneName));
    }

    private IEnumerator LoadSceneByNameCoroutine(string sceneName)
    {
        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(1);
        yield return new WaitUntil(() => loadingOperation.isDone);

        yield return new WaitForSeconds(0.3f);

        AsyncOperation targetOperation = SceneManager.LoadSceneAsync(sceneName);

        while (!targetOperation.isDone)
        {
            float progress = Mathf.Clamp01(targetOperation.progress / 0.9f);
            LoadingScreen.Instance?.UpdateProgress(progress);
            yield return null;
        }
        
        Time.timeScale = 1f;
    }

    private void LoadSceneAsync(int sceneIndex)
    {
        StartCoroutine(LoadSceneCoroutine(sceneIndex));
    }

    private IEnumerator LoadSceneCoroutine(int sceneIndex)
    {
        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(1);
        yield return new WaitUntil(() => loadingOperation.isDone);

        AsyncOperation targetOperation = SceneManager.LoadSceneAsync(sceneIndex);
        
        while (!targetOperation.isDone)
        {
            float progress = Mathf.Clamp01(targetOperation.progress / 0.9f);
            LoadingScreen.Instance?.UpdateProgress(progress);
            yield return null;
        }
        
        Time.timeScale = 1f;
    }
}