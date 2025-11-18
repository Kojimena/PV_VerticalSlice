using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class SpaceshipRepair : MonoBehaviour
{
    [Header("Pieces")]
    [SerializeField] private List<string> requiredPieces = new List<string>
    {
        "Tail #1",
        "Tail #2",
        "Tail #3",
        "Wing #1 L",
        "Wing #1 R",    
        "Wing #2 L",
        "Wing #2 R",
        "Wing #3 L",
        "Wing #3 R",
    };

    [SerializeField] private GameObject repairedSpaceshipPrefab;

    [Header("UI")]
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private float successMessageTime = 3f;

    [Header("Audio")]
    [SerializeField] private AudioClip incompleteSound;
    [SerializeField] private AudioClip completeSound;

    private bool playerNearby = false;
    private bool isRepaired = false;

    void Start()
    {
        if (statusText) statusText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerNearby && !isRepaired)
        {
            CheckRepairStatus();
        }
    }

    private void CheckRepairStatus()
    {
        List<string> missingPieces = new List<string>();

        foreach (string piece in requiredPieces)
        {
            if (!HasItemInInventory(piece))
            {
                missingPieces.Add(piece);
            }
        }

        if (missingPieces.Count > 0)
        {
            ShowMissingPiecesMessage(missingPieces);
            if (incompleteSound != null)
            {
                AudioManager.instance?.PlaySFX(incompleteSound);
            }
        }
        else
        {
            RepairSpaceship();
        }
    }

    private bool HasItemInInventory(string itemName)
    {
        if (PersistenceManager.Instance == null) return false;

        foreach (var item in PersistenceManager.Instance.data.inventoryItems)
        {
            if (item.itemName == itemName && item.quantity > 0)
            {
                return true;
            }
        }

        return false;
    }

    private void ShowMissingPiecesMessage(List<string> missingPieces)
    {
        if (statusText == null) return;

        string message = $"Missing pieces ({missingPieces.Count}/{requiredPieces.Count}):\n";
        
        foreach (string piece in missingPieces)
        {
            message += $"- {piece}\n";
        }

        statusText.gameObject.SetActive(true);
        statusText.text = message;

        StopAllCoroutines();
    }

    private void RepairSpaceship()
    {
        isRepaired = true;
        
        if (statusText)
        {
            statusText.text = "The spaceship has been repaired!";
            statusText.gameObject.SetActive(true);
            statusText.alignment = TextAlignmentOptions.Center;
        }

        if (completeSound != null)
        {
            AudioManager.instance?.PlaySFX(completeSound);
        }

        StartCoroutine(CompleteRepairSequence());
    }

    private IEnumerator CompleteRepairSequence()
    {
        yield return new WaitForSeconds(successMessageTime);

        if (repairedSpaceshipPrefab != null)
        {
            repairedSpaceshipPrefab.SetActive(true);
        }


        if (statusText) statusText.gameObject.SetActive(false);
        gameObject.SetActive(false);
        
        GameManager.Instance.LoadSceneByName("WinMenu");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            if (statusText && !isRepaired) statusText.gameObject.SetActive(false);
            if (!isRepaired) StopAllCoroutines();
        }
    }
}