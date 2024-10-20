using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResidentInfoUIController : MonoBehaviour
{
    public GameObject residentInfoPanel;
    public TMP_Text nameText; // TextMeshPro Text
    public TMP_Text moodText; // TextMeshPro Text
    public TMP_Text assetsText; // TextMeshPro Text
    public Button closeButton;

    private void Start()
    {
        //closeButton.onClick.AddListener(ClosePanel);
    }

    public void ShowResidentInfo(Resident resident)
    {
        residentInfoPanel.SetActive(true);
        nameText.text = $"Name: {resident.residentName}";
        moodText.text = $"Mood: {resident.mood}";
        assetsText.text = $"Food: {resident.assets}";
    }

    private void ClosePanel()
    {
        residentInfoPanel.SetActive(false);
    }
}
