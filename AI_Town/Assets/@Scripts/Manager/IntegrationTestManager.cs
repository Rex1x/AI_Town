using UnityEngine;
using UnityEngine.UI;

public class IntegrationTestManager : MonoBehaviour
{
    public ResidentManager residentManager;
    public GodWordUIController godWordUIController;
    public TimeManager timeManager;
    public Button generateResidentButton;
    public Button triggerCustomEventButton;
    public Button skipDayButton;

    private void Start()
    {
        // 각 버튼의 기능 연결
        generateResidentButton.onClick.AddListener(() => GenerateNewResident());
        //triggerCustomEventButton.onClick.AddListener(() => TriggerCustomEvent());
        skipDayButton.onClick.AddListener(() => SkipToNextDay());

        // 신의 말 테스트: 기본 메시지 설정
        godWordUIController.godWordInputField.text = "Work hard today!";
        godWordUIController.submitButton.onClick.AddListener(() => godWordUIController.OnSubmitGodWord());
    }

    // 새로운 Resident 생성 테스트
    private async void GenerateNewResident()
    {
        await residentManager.GenerateAndShowResidents();
        Debug.Log("New resident generated.");
    }

    // 사용자 정의 이벤트 트리거 테스트
    private void TriggerCustomEvent()
    {
        FindObjectOfType<EventManager>().TriggerCustomEvent("Festival");
        Debug.Log("Custom event triggered: Festival.");
    }

    // 하루 스킵 테스트
    private void SkipToNextDay()
    {
        timeManager.MoveToNextTimePeriod();
        timeManager.MoveToNextTimePeriod();
        timeManager.MoveToNextTimePeriod();
        Debug.Log("Skipped to the next day.");
    }

    private void Update()
    {
        // 현재 시간을 화면에 표시
        timeManager.Update();
    }
}
