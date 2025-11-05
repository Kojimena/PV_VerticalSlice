using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{

    private void Awake()
    {
        GameObject obj = this.gameObject;

        if (PersistenceManager.Instance != null &&
            PersistenceManager.Instance.data != null &&
            PersistenceManager.Instance.data.isDoorOpen)
        {
            obj.SetActive(false);
        }
        else
        {
            obj.SetActive(true);
        }
    }
}