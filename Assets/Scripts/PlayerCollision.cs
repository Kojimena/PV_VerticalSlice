using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("door"))
        {
            GameEventsBehaviour.Instance.RaiseDoorEntered();
        }
    }
}
