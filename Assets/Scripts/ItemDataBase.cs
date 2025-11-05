using System.Collections.Generic;
using UnityEngine;

public class ItemDataBase : MonoBehaviour
{
    public static ItemDataBase Instance;
    
    [Header("All Items in Game")]
    [SerializeField] private List<PickUpData> allItems = new List<PickUpData>();
    
    private Dictionary<string, PickUpData> itemLookup = new Dictionary<string, PickUpData>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDatabase();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeDatabase()
    {
        itemLookup.Clear();
        
        foreach (var item in allItems)
        {
            if (item != null && !string.IsNullOrEmpty(item.displayName))
            {
                if (!itemLookup.ContainsKey(item.displayName))
                {
                    itemLookup[item.displayName] = item;
                }
                else
                {
                    Debug.LogWarning($"<color=orange>[ItemDatabase]</color> Item duplicado: {item.displayName}");
                }
            }
        }
        
    }
    
    public PickUpData GetItemByName(string itemName)
    {
        if (itemLookup.TryGetValue(itemName, out PickUpData item))
        {
            return item;
        }
        
        Debug.LogWarning($"<color=orange>[ItemDatabase]</color> Item no encontrado: {itemName}");
        return null;
    }
    
    public bool HasItem(string itemName)
    {
        return itemLookup.ContainsKey(itemName);
    }
}