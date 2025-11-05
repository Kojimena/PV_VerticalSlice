using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public int currentLives = 3;
    public int maxLives = 3;
    public int coinCount = 0;
    public List<InventoryItemData> inventoryItems = new List<InventoryItemData>();
    public Vector3 playerPosition;
    public string currentSceneName;
    public List<string> collectedItemIds = new List<string>();
    public bool isDoorOpen = false;
    public bool hasCheckpoint = false;

    
    public void ResetToDefault()
    {
        currentLives = 3;
        maxLives = 3;
        coinCount = 0;
        inventoryItems = new List<InventoryItemData>();
        playerPosition = Vector3.zero;
        currentSceneName = string.Empty;
        collectedItemIds = new List<string>();
        isDoorOpen = false;
        hasCheckpoint = false; 
    }
}



[Serializable]
public class InventoryItemData
{
    public string itemName;
    public int quantity;
    public bool stackable;

    public InventoryItemData(string name, int qty, bool stack)
    {
        itemName = name;
        quantity = qty;
        stackable = stack;
    }
}