using UnityEngine;

public class HeartBehaviour : MonoBehaviour
{
    [SerializeField] private HealthItemData healthData;
    private PersistentItem persistentItem;

    private void Awake()
    {
        persistentItem = GetComponent<PersistentItem>();
        
        if (persistentItem == null)
        {
            Debug.LogWarning($"<color=orange>[HeartBehaviour]</color> {gameObject.name} no tiene componente PersistentItem!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log($"<color=cyan>[HeartBehaviour]</color> Corazón recogido: {gameObject.name}");

        if (persistentItem != null)
        {
            persistentItem.MarkAsCollected();
        }

        if (GameEventsBehaviour.Instance != null)
        {
            GameEventsBehaviour.Instance.RaiseLifeCollected();
        }

        if (healthData != null && healthData.pickUpSound != null)
        {
            AudioManager.instance.PlayAudioClip(healthData.pickUpSound);
        }

        Destroy(gameObject);
    }
}