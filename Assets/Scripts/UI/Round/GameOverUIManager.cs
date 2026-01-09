using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUIManager : MonoBehaviour
{
    [SerializeField] private Canvas gameOverCanvas;
    [SerializeField] private TextMeshProUGUI flavorText;
    [SerializeField] private GraphicRaycaster raycaster;
    void Start() 
    {
        if (gameOverCanvas == null) gameOverCanvas = GetComponentInParent<Canvas>();
        raycaster = gameOverCanvas.GetComponent<GraphicRaycaster>();
        if (flavorText == null) flavorText = gameOverCanvas.GetComponentInChildren<TextMeshProUGUI>();
        gameOverCanvas.enabled = false;
        raycaster.enabled = false;
    }
    public void EnableUI(string flavorText)
    {
        gameOverCanvas.enabled = true;
        this.flavorText.text = flavorText;
        raycaster.enabled = true;
    }
    public void EnableUI() {
        EnableUI("You have failed. How unfortunate.");
    }

    public void DisableUI()
    {
        gameOverCanvas.enabled = false;
        raycaster.enabled = false;
    }

}