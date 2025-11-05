using UnityEngine;

public class CoinBehaviour : MonoBehaviour
{
    [SerializeField] private ValuedItemData coinData;
    private PersistentItem persistentItem;
   
    private void Awake()
    {
        persistentItem = GetComponent<PersistentItem>();
        
        if (persistentItem == null)
        {
            Debug.LogWarning($"<color=orange>[CoinBehaviour]</color> {gameObject.name} no tiene componente PersistentItem!");
        }
    }
   
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log($"<color=cyan>[CoinBehaviour]</color> Moneda recogida: {gameObject.name}");

        if (persistentItem != null)
        {
            persistentItem.MarkAsCollected();
        }

        if (GameEventsBehaviour.Instance != null)
        {
            GameEventsBehaviour.Instance.RaiseCoinCollected();
        }

        if (coinData != null && coinData.pickUpSound != null)
        {
            AudioManager.instance.PlayAudioClip(coinData.pickUpSound);
        }

        Destroy(gameObject);
    }
}