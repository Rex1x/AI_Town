using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GodWordUIController : MonoBehaviour
{
    public TMP_InputField godWordInputField; // TextMeshPro InputField
    public Button submitButton;
    public TMP_Text feedbackText; // TextMeshPro Text


    public TMP_Text timePeriodText;
    public TMP_Text dayCounterText;

    private int currentDay = 1;
    public ResidentManager residentManager; // ResidentManager 참조 추가


    private void Start()
    {
        submitButton.onClick.AddListener(OnSubmitGodWord);
        TimeManager.OnTimePeriodChanged += UpdateTimePeriodUI;
    }

    public async void OnSubmitGodWord()
    {
        string godWord = godWordInputField.text;
        if (string.IsNullOrEmpty(godWord))
        {
            feedbackText.text = "Please enter a message from the god.";
            return;
        }

        feedbackText.text = "Sending God's word...";

        // 신의 말을 각 주민에게 전달
        Resident[] residents = residentManager.GetAllResidents();
        if (residents.Length == 0)
        {
            feedbackText.text = "No residents available to receive God's word.";
            return;
        }


        foreach (var resident in residents)
        {
            //await resident.DecideNextActionAsync();
            //await resident.ReactToGodWordAsync(godWord);
        }

        feedbackText.text = "God's word has been delivered.";
    }

    private void UpdateTimePeriodUI(TimeManager.TimeOfDay timeOfDay)
    {
        switch (timeOfDay)
        {
            case TimeManager.TimeOfDay.Morning:
                timePeriodText.text = "Morning";
                break;
            case TimeManager.TimeOfDay.Afternoon:
                timePeriodText.text = "Afternoon";
                break;
            case TimeManager.TimeOfDay.Evening:
                timePeriodText.text = "Evening";
                currentDay++;
                dayCounterText.text = $"Day {currentDay}";
                break;
        }
    }
}
