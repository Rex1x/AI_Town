using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private List<IEvent> activeEvents = new List<IEvent>();

    // 플레이어가 직접 추가하는 사용자 정의 이벤트
    public void TriggerCustomEvent(string eventName)
    {
        IEvent customEvent = new CustomEvent(eventName);
        activeEvents.Add(customEvent);
        customEvent.Execute();
        Debug.Log($"Custom Event Triggered: {eventName}");
    }

    // 자동 발생하는 환경적 이벤트 추가
    public void AddEnvironmentalEvent(string eventName, int duration)
    {
        IEvent envEvent = new EnvironmentalEvent(eventName, duration);
        activeEvents.Add(envEvent);
        envEvent.Execute();
        Debug.Log($"Environmental Event Added: {eventName}, Duration: {duration} days");
    }

    // 활성 이벤트 관리
    public void Update()
    {
        foreach (var evt in activeEvents)
        {
            evt.UpdateEvent();
        }

        // 완료된 이벤트 제거
        activeEvents.RemoveAll(evt => evt.IsCompleted());
    }
}

public class CustomEvent : IEvent
{
    private string eventName;
    private bool completed = false;

    public CustomEvent(string eventName)
    {
        this.eventName = eventName;
    }

    public void Execute()
    {
        // 사용자 정의 이벤트 실행 로직
        Debug.Log($"Executing Custom Event: {eventName}");
        // 예: 주민들에게 특정 자산 추가하기
        Resident[] residents = DataManager.LoadAllResidents();
        foreach (var resident in residents)
        {
            resident.assets += 50;
        }

        completed = true; // 이벤트 완료 처리
    }

    public void UpdateEvent() { }

    public bool IsCompleted()
    {
        return completed;
    }
}


public class EnvironmentalEvent : IEvent
{
    private string eventName;
    private int duration; // 이벤트 지속 시간 (일 단위)
    private int elapsedDays = 0;
    private bool completed = false;

    public EnvironmentalEvent(string eventName, int duration)
    {
        this.eventName = eventName;
        this.duration = duration;
    }

    public void Execute()
    {
        Debug.Log($"Starting Environmental Event: {eventName}");
        // 예: 폭염, 질병 발생 등
        // 주민의 상태 변화 등을 적용
    }

    public void UpdateEvent()
    {
        elapsedDays++;
        if (elapsedDays >= duration)
        {
            Debug.Log($"Environmental Event Completed: {eventName}");
            completed = true;
        }
    }

    public bool IsCompleted()
    {
        return completed;
    }
}

public interface IEvent
{
    void Execute(); // 이벤트 실행
    void UpdateEvent(); // 매 프레임마다 이벤트 업데이트
    bool IsCompleted(); // 이벤트 완료 여부 체크
}
