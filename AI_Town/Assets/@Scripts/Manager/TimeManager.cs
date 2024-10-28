using UnityEngine;
using System.Collections.Generic;

public class TimeManager : MonoBehaviour
{
    public enum TimeOfDay { Morning, Afternoon, Evening }
    public TimeOfDay currentTime;
    public float realTimeMinutesPerDay = 1.0f; // 현실 시간 10분을 하루로 설정

    private float elapsedTime;
    private List<Resident> residents = new List<Resident>();

    public delegate void TimePeriodChanged(TimeOfDay newTime);
    public static event TimePeriodChanged OnTimePeriodChanged;

    private void Start()
    {
        currentTime = TimeOfDay.Morning;
        elapsedTime = 0f;

        // 초기 주민 목록 로드
        residents.AddRange(FindObjectOfType<ResidentManager>().GetAllResidents());
    }

    public void Update()
    {
        //elapsedTime += Time.deltaTime;

        //// 하루의 시간대 계산
        //float timePerPeriod = (realTimeMinutesPerDay * 60f) / 3; // 아침, 점심, 저녁 3번의 변화

        //if (elapsedTime > timePerPeriod)
        //{
        //    MoveToNextTimePeriod();
        //    elapsedTime = 0f;
        //}
    }

    public void MoveToNextTimePeriod()
    {
        currentTime = (TimeOfDay)(((int)currentTime + 1) % 3);

        // 현재 시간대에 따라 AI 주민의 활동 결정
        foreach (var resident in residents)
        {
            resident.DecideNextActionAsync(); // AI 호출
        }

        OnTimePeriodChanged?.Invoke(currentTime);
    }
}
