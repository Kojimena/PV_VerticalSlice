using UnityEngine;

public class ItemInventoryBehaviour : MonoBehaviour
{
    [SerializeField] private PickUpData pickUpData;
    private PersistentItem persistentItem;

    private void Awake()
    {
        persistentItem = GetComponent<PersistentItem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Marcar como recogido
        if (persistentItem != null)
        {
            persistentItem.MarkAsCollected();
        }

        // Agregar al inventario
        if (pickUpData.goesToInventory)
        {
            GameEventsBehaviour.Instance.RaiseItemInventoryCollected(pickUpData); 
        }

        // Reproducir sonido
        if (pickUpData.pickUpSound != null)
        {
            AudioManager.instance.PlayAudioClip(pickUpData.pickUpSound);
        }

        // Destruir
        Destroy(gameObject);
    }
}