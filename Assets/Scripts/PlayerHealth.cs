using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxLives = 10;
    private int currentLives;
    [SerializeField] private float fallThreshold = -10f;
    
    private void OnEnable()
    {
        if (GameEventsBehaviour.Instance != null)
            GameEventsBehaviour.Instance.OnLifeCollected += OnLifePickup;
    }
    
    private void OnDisable()
    {
        if (GameEventsBehaviour.Instance != null)
            GameEventsBehaviour.Instance.OnLifeCollected -= OnLifePickup;
    }
    
    void Start()
    {
        if (PersistenceManager.Instance != null && PersistenceManager.Instance.data != null)
        {
            maxLives = PersistenceManager.Instance.data.maxLives;
            currentLives = PersistenceManager.Instance.data.currentLives;
        }
        else
        {
            currentLives = maxLives;
        }
        
        // Notificar UI
        if (GameEventsBehaviour.Instance != null)
        {
            GameEventsBehaviour.Instance.RaiseLivesChanged(currentLives, maxLives);
        }
    }
    
    void Update()
    {
        if (transform.position.y < fallThreshold)
        {
            Die();
        }
        
    }
    
    public void Heal(int amount)
    {
        if (amount <= 0) return;
        
        currentLives = Mathf.Clamp(currentLives + amount, 0, maxLives);
        
        Debug.Log($"<color=green>[PlayerHealth]</color> Curando {amount} vidas. Total: {currentLives}/{maxLives}");
        
        if (GameEventsBehaviour.Instance != null)
        {
            GameEventsBehaviour.Instance.RaiseLivesChanged(currentLives, maxLives);
        }
        
        // persistencia
        if (PersistenceManager.Instance != null)
        {
            PersistenceManager.Instance.data.currentLives = currentLives;
        }
    }

    public void TakeDamage(int amount)
    {
        currentLives = Mathf.Clamp(currentLives - amount, 0, maxLives);
        
        Debug.Log($"<color=red>[PlayerHealth]</color> Daño recibido: {amount}. Vidas restantes: {currentLives}/{maxLives}");
        
        if (GameEventsBehaviour.Instance != null)
        {
            GameEventsBehaviour.Instance.RaiseLivesChanged(currentLives, maxLives);
        }
        
        if (PersistenceManager.Instance != null)
        {
            PersistenceManager.Instance.data.currentLives = currentLives;
        }

        if (currentLives <= 0)
        {
            Die();
        }
    }
    
    private void OnLifePickup()
    {
        currentLives = Mathf.Clamp(currentLives + 1, 0, maxLives);
        
        Debug.Log($"<color=lime>[PlayerHealth]</color> Vida recogida! Total: {currentLives}/{maxLives}");
        
        if (GameEventsBehaviour.Instance != null)
        {
            GameEventsBehaviour.Instance.RaiseLivesChanged(currentLives, maxLives);
        }
        
        //  persistencia
        if (PersistenceManager.Instance != null)
        {
            PersistenceManager.Instance.data.currentLives = currentLives;
        }
    }
    
    private void Die()
    {
        gameObject.SetActive(false); 
        SceneManager.LoadScene("LostMenu");
    }
    
    public int GetCurrentLives()
    {
        return currentLives;
    }
    
    public void SetCurrentLives(int lives)
    {
        currentLives = Mathf.Clamp(lives, 0, maxLives);
        
        if (GameEventsBehaviour.Instance != null)
        {
            GameEventsBehaviour.Instance.RaiseLivesChanged(currentLives, maxLives);
        }
    }
}