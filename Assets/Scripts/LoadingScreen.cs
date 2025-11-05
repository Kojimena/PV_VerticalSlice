using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] Image progressImage;
    public TextMeshProUGUI loadingText;
    

    void Awake()
    {
        Instance = this;
    }
    

    public void UpdateProgress(float progress)
    {
        if (progressImage)
        {
            progressImage.fillAmount = progress;
        }
        
        if (loadingText)
        {
            int percent = Mathf.RoundToInt(progress * 100f);
            loadingText.text = $"Loading... {percent}%";
        }
    }
}