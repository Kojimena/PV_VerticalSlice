using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void BeginNewAdventure()
    {
        if (PersistenceManager.Instance != null)
        {
            PersistenceManager.Instance.data = new GameData();
            PersistenceManager.Instance.data.ResetToDefault();
            PersistenceManager.Instance.SaveSessionData(PersistenceManager.Instance.data);
        }

        LoadLevelWithLoading(1);
    }

    public void ContinueGame()
    {
        if (PersistenceManager.Instance == null)
        {
            Debug.LogError("No PersistenceManager found in scene!");
            return;
        }

        string sceneToLoad = PersistenceManager.Instance.data.currentSceneName;

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadSceneByName(sceneToLoad);
            }
            else
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
        else
        {
            Debug.LogWarning("No saved scene found, starting a new game instead.");
            BeginNewAdventure();
        }
    }

    public void LoadLevelFromMenu(int levelNumber)
    {
        LoadLevelWithLoading(levelNumber);
    }

    public void ExitGame()
    {
        Application.Quit();
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    private void LoadLevelWithLoading(int levelNumber)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadLevel(levelNumber);
        }
        else
        {
            Debug.LogWarning("GameManager not found, loading scene directly");
            SceneManager.LoadScene($"level{levelNumber}");
        }
    }
}