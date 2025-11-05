using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PersistenceManager : MonoBehaviour
{
    public static PersistenceManager Instance;
    public GameData data;
    string saveFilePath;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            data = new GameData();
            saveFilePath = Application.persistentDataPath + "/savefile.json";
            LoadSessionData(data);

        }
        else
        {
            Destroy(gameObject);
        }
        
    }
    
    public void SaveSessionData(GameData session)
    {
        GameData sessionData = new GameData
        {
            currentLives = session.currentLives,
            maxLives = session.maxLives,
            coinCount = session.coinCount,
            inventoryItems = session.inventoryItems,
            playerPosition = session.playerPosition,
            currentSceneName = SceneManager.GetActiveScene().name,
            collectedItemIds = session.collectedItemIds ,
            hasCheckpoint = session.hasCheckpoint,
            isDoorOpen = session.isDoorOpen
        };
        
        try
        {
            string json = JsonUtility.ToJson(sessionData, true);
            File.WriteAllText(saveFilePath, json);
            Debug.Log("Game saved to " + saveFilePath);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save game: " + e.Message);
        }
    }
    
    private void OnApplicationQuit()
    {
        SaveSessionData(data);
    }
    
    public void LoadSessionData(GameData session)
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("No save file found at " + saveFilePath);
            return;
        }
    
        try
        {
            string json = File.ReadAllText(saveFilePath);
            GameData loadedData = JsonUtility.FromJson<GameData>(json);
            session.currentLives = loadedData.currentLives;
            session.maxLives = loadedData.maxLives;
            session.coinCount = loadedData.coinCount;
            session.inventoryItems = loadedData.inventoryItems ?? new System.Collections.Generic.List<InventoryItemData>();
            session.playerPosition = loadedData.playerPosition;
            session.currentSceneName = loadedData.currentSceneName;
            session.collectedItemIds = loadedData.collectedItemIds ?? new System.Collections.Generic.List<string>();
            session.hasCheckpoint = loadedData.hasCheckpoint;
            session.isDoorOpen = loadedData.isDoorOpen;


        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load game: " + e.Message);
        }
    }
    
    public void SavePlayerSnapshot(Transform player)
    {
        if (player == null) return;
        data.playerPosition = new Vector3(player.position.x, player.position.y, player.position.z);
        data.currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        data.hasCheckpoint    = true;

        SaveSessionData(data); 
    }

}
