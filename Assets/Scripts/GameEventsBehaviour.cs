using UnityEngine;
using System;
using UnityEngine.Events;

[DefaultExecutionOrder(-1000)]

public class GameEventsBehaviour : MonoBehaviour
{
    
    public static GameEventsBehaviour Instance;
    public event Action OnCoinCollected; // Evento para cuando se recoge una moneda
    public event Action OnLifeCollected; // Evento para cuando se recoge una vida
    public event Action<int,int> OnLivesChanged; // Evento para cuando cambian las vidas
    
    public event Action<PickUpData> OnItemInventoryCollected; // Evento para cuando se recoge un item de inventario
    
    public event Action OnDoorEntered; // Evento para cuando se entra en una puerta

    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void RaiseCoinCollected()
    {
        OnCoinCollected?.Invoke();
    }
    
    public void RaiseLifeCollected()
    {
        OnLifeCollected?.Invoke();
    }
    
    public void RaiseLivesChanged(int currentLives, int maxLives)
    {
        OnLivesChanged?.Invoke(currentLives, maxLives);
    }
    
    
    public void RaiseItemInventoryCollected(PickUpData itemData)
    {
        OnItemInventoryCollected?.Invoke(itemData);
    }
    
    public void RaiseDoorEntered()
    {
        OnDoorEntered?.Invoke();
    }
    
    
}
