using UnityEngine;

[CreateAssetMenu(fileName = "PickUpData", menuName = "Items/PickUp")]
public class PickUpData : ScriptableObject
{
    public string displayName;
    public Sprite icon;
    public AudioClip pickUpSound;


    [Header("Opciones")]
    public bool goesToInventory = true;
    public bool stackable = true;

}