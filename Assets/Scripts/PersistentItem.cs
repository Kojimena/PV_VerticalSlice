using UnityEngine;
public class PersistentItem : MonoBehaviour
{
    [Header("Item Unique ID")]
    public string itemId;
    
    [Header("Auto-generate ID")]
    [SerializeField] private bool autoGenerateId = true;

    private void Awake()
    {
        // Generar ID único si no existe
        if (string.IsNullOrEmpty(itemId) || autoGenerateId)
        {
            GenerateUniqueId();
        }
        
        // Verificar si este item ya fue recogido
        if (WasCollected())
        {
            Destroy(gameObject);
        }
    }

    private void GenerateUniqueId()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Vector3 pos = transform.position;
        
        itemId = $"{sceneName}_{gameObject.name}_{pos.x:F2}_{pos.y:F2}_{pos.z:F2}";
    }

   
    private bool WasCollected()
    {
        if (PersistenceManager.Instance == null) return false;
        
        return PersistenceManager.Instance.data.collectedItemIds.Contains(itemId);
    }


    public void MarkAsCollected()
    {
        if (PersistenceManager.Instance == null) return;
        
        if (!PersistenceManager.Instance.data.collectedItemIds.Contains(itemId))
        {
            PersistenceManager.Instance.data.collectedItemIds.Add(itemId);
            PersistenceManager.Instance.SaveSessionData(PersistenceManager.Instance.data);
            
        }
    }

}