using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI References")]
    [SerializeField] private TMP_Text coinText;
   
    [Header("Lives UI")]
    [SerializeField] private Transform livesPanel;
    [SerializeField] private GameObject heartPrefab;
    
    [Header("Door UI")]
    [SerializeField] private TMP_Text doorText;
    [SerializeField] private GameObject doorObject;
    
    [Header("Inventory UI")]
    [SerializeField] private Transform inventoryContent; 
    [SerializeField] private GameObject inventoryItemPrefab; 
    
    private int coinCount;
    
    private readonly Dictionary<PickUpData, int> inventoryStacks = new();
    private readonly Dictionary<PickUpData, GameObject> inventoryRows = new();
    
    private PickUpData GetPickUpByName(string itemName)
    {
        return ItemDataBase.Instance.GetItemByName(itemName);
    }
    
    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
    private void Start()
    {
        coinCount = PersistenceManager.Instance.data.coinCount;
        if (coinText) coinText.text = $"Coins: {coinCount}";
        
        //load inventory
        foreach (var itemData in PersistenceManager.Instance.data.inventoryItems)
        {
            var data = GetPickUpByName(itemData.itemName);
            if (data != null)
            {
                if (data.stackable)
                {
                    inventoryStacks[data] = itemData.quantity;
                    CreateRow(data, itemData.quantity);
                }
                else
                {
                    for (int i = 0; i < itemData.quantity; i++)
                    {
                        CreateRow(data, 1, forceNewRow:true);
                    }
                }
            }
            else
            {
                Debug.LogWarning($"<color=orange>[UIManager]</color> Item no encontrado en catálogo: {itemData.itemName}");
            }
        }
    }

    private void OnEnable()
    {
        if (GameEventsBehaviour.Instance != null)
        {
            GameEventsBehaviour.Instance.OnCoinCollected += UpdateCoins;
            GameEventsBehaviour.Instance.OnLivesChanged += UpdateLivesUI;
            GameEventsBehaviour.Instance.OnItemInventoryCollected += AddToInventory;
            GameEventsBehaviour.Instance.OnDoorEntered += UpdateDoorUI;
        }
    }

    private void OnDisable()
    {
        if (GameEventsBehaviour.Instance != null)
        {
            GameEventsBehaviour.Instance.OnCoinCollected -= UpdateCoins;
            GameEventsBehaviour.Instance.OnLivesChanged -= UpdateLivesUI;
            GameEventsBehaviour.Instance.OnItemInventoryCollected -= AddToInventory;
            GameEventsBehaviour.Instance.OnDoorEntered -= UpdateDoorUI;
        }
    }
    
    private void UpdateDoorUI()
    {
        if (HasItemByName("Key"))
        {
            if (doorObject) doorObject.SetActive(false);
            if (doorText) doorText.text = string.Empty;
            
            if (PersistenceManager.Instance != null)
            {
                PersistenceManager.Instance.data.isDoorOpen = true;
                PersistenceManager.Instance.SaveSessionData(PersistenceManager.Instance.data);
            }
        }
        else
        {
            if (doorText != null)
            {
                doorText.gameObject.SetActive(true);
                doorText.text = "Necesitas una llave para abrir la puerta";
                StopAllCoroutines(); 
                StartCoroutine(HideDoorTextAfter(2.5f)); 
            }
        }
    }
    
    private System.Collections.IEnumerator HideDoorTextAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (doorText) doorText.text = string.Empty;
    }

    public bool HasItemByName(string itemName)
    {
        foreach (var kvp in inventoryStacks)
        {
            if (kvp.Key.displayName == itemName && kvp.Value > 0)
            {
                return true;
            }
        }

        foreach (var row in inventoryRows.Keys)
        {
            if (row.displayName == itemName)
            {
                return true;
            }
        }

        return false;
    }
    
    private void UpdateCoins()
    {
        int value = HasItemByName("Magnet") ? 2 : 1;
        coinCount += value;

        if (coinText) coinText.text = $"Coins: {coinCount}";
        
        if (PersistenceManager.Instance != null)
        {
            PersistenceManager.Instance.data.coinCount = coinCount;
        }
    }
    
    private void UpdateLivesUI(int currentLives, int maxLives)
    {
        if (livesPanel == null) return;

        for (int i = livesPanel.childCount - 1; i >= 0; i--)
        {
            var child = livesPanel.GetChild(i);
            if (child) Destroy(child.gameObject);
        }

        for (int i = 0; i < currentLives; i++)
        {
            if (heartPrefab) Instantiate(heartPrefab, livesPanel);
        }
        
        if (PersistenceManager.Instance != null)
        {
            PersistenceManager.Instance.data.currentLives = currentLives;
            PersistenceManager.Instance.data.maxLives = maxLives;
        }
    }
    
    private void AddToInventory(PickUpData data)
    {
        if (data == null) return;

        if (!data.stackable)
        {
            CreateRow(data, 1, forceNewRow:true);
            SaveInventoryToPersistence();
            return;
        }

        if (inventoryStacks.TryGetValue(data, out int current))
        {
            current++;
            inventoryStacks[data] = current;
            UpdateRow(data, current);
        }
        else
        {
            inventoryStacks[data] = 1;
            CreateRow(data, 1);
        }
        
        SaveInventoryToPersistence();
    }
    
    private void SaveInventoryToPersistence()
    {
        if (PersistenceManager.Instance == null) return;
        
        List<InventoryItemData> inventoryList = new List<InventoryItemData>();
        
        foreach (var kvp in inventoryStacks)
        {
            if (kvp.Key != null && kvp.Value > 0)
            {
                inventoryList.Add(new InventoryItemData(
                    kvp.Key.displayName,
                    kvp.Value,
                    kvp.Key.stackable
                ));
            }
        }
        
        foreach (var data in inventoryRows.Keys)
        {
            if (data != null && !inventoryStacks.ContainsKey(data))
            {
                inventoryList.Add(new InventoryItemData(
                    data.displayName,
                    1,
                    data.stackable
                ));
            }
        }
        
        PersistenceManager.Instance.data.inventoryItems = inventoryList;
    }

    private void CreateRow(PickUpData data, int amount, bool forceNewRow = false)
    {
        if (!forceNewRow && inventoryRows.TryGetValue(data, out var existing))
        {
            UpdateRow(data, amount);
            return;
        }

        var row = Instantiate(inventoryItemPrefab, inventoryContent);
        inventoryRows[data] = row;

        var icon = row.GetComponentInChildren<Image>(true);
        var label = row.GetComponentInChildren<TMP_Text>(true);

        if (icon) icon.sprite = data.icon;
        if (label) label.text = BuildItemLabel(data.displayName, amount);
    }

    private void UpdateRow(PickUpData data, int amount)
    {
        if (!inventoryRows.TryGetValue(data, out var row)) return;

        var label = row.GetComponentInChildren<TMP_Text>(true);
        if (label) label.text = BuildItemLabel(data.displayName, amount);
    }

    private string BuildItemLabel(string nameitem, int amount)
    {
        return amount > 1 ? $"{nameitem} x{amount}" : nameitem;
    }
    
    public bool TryConsumeItem(string itemName, out int itemValue)
    {
        itemValue = 0;

        PickUpData foundStackable = null;
        int currentAmount = 0;

        foreach (var kvp in inventoryStacks)
        {
            if (kvp.Key.displayName == itemName && kvp.Value > 0)
            {
                foundStackable = kvp.Key;
                currentAmount = kvp.Value;
                break;
            }
        }

        if (foundStackable != null)
        {
            if (foundStackable is HealthItemData valued)
                itemValue = valued.healthRestored;

            int newAmount = currentAmount - 1;

            if (newAmount > 0)
            {
                inventoryStacks[foundStackable] = newAmount;
                UpdateRow(foundStackable, newAmount);
            }
            else
            {
                inventoryStacks.Remove(foundStackable);
                if (inventoryRows.TryGetValue(foundStackable, out var row) && row)
                    Destroy(row);
                inventoryRows.Remove(foundStackable);
            }
            
            SaveInventoryToPersistence();
            return true;
        }

        PickUpData foundNonStack = null;
        foreach (var data in inventoryRows.Keys)
        {
            if (data.displayName == itemName)
            {
                foundNonStack = data;
                break;
            }
        }

        if (foundNonStack != null)
        {
            if (foundNonStack is ValuedItemData valued2)
                itemValue = valued2.value;

            if (inventoryRows.TryGetValue(foundNonStack, out var row2) && row2)
                Destroy(row2);
            inventoryRows.Remove(foundNonStack);

            SaveInventoryToPersistence();
            return true;
        }

        return false;
    }
}