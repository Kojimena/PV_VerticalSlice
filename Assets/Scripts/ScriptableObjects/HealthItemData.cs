using UnityEngine;


[CreateAssetMenu(fileName = "NewHealthItem", menuName = "Items/Health Item")]
public class HealthItemData : PickUpData
{
    public int healthRestored;
}
