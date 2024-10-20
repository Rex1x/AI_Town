using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ResidentObjectController : MonoBehaviour
{

    public TMP_Text nameText;
    public TMP_Text moodText;
    public TMP_Text assetsText;
    public SpriteRenderer moodSprite; // SpriteRenderer로 유지

    // 표정 스프라이트를 저장할 Dictionary
    public Dictionary<string, Sprite> emotionSprites;

    public Sprite BaseMode;

    private Resident resident;

    // Resident 데이터를 기반으로 오브젝트 초기화
    public void Initialize(Resident residentData)
    {
        resident = residentData;
        emotionSprites = ResidentManager.emotionSprites;
        UpdateResidentInfo();
    }

    // 표정 스프라이트 추가 메서드
    public void AddEmotionSprite(string emotionName, Sprite sprite)
    {
        string key = emotionName.ToLower();
        if (!emotionSprites.ContainsKey(key))
        {
            emotionSprites.Add(key, sprite);
            //Debug.Log($"Added emotion sprite: {key}");
        }
    }

    private void UpdateResidentInfo()
    {
        // 텍스트 업데이트
        nameText.text = resident.residentName;
        moodText.text = $"Mood: {resident.mood}";
        assetsText.text = $"Assets: {resident.assets}";

        // 기분에 따른 표정 업데이트
        UpdateMoodSprite(resident.mood);
    }

    // 기분에 따른 스프라이트 업데이트
    private void UpdateMoodSprite(string mood)
    {
        string moodKey = mood.ToLower();
        Debug.Log("Mood ? : " + moodKey);
        if (emotionSprites.ContainsKey(moodKey))
        {
            moodSprite.sprite = emotionSprites[moodKey];
            Debug.Log("Check Key Yes");
        }
        else
        {
            //moodSprite.sprite = BaseMode; // 기본 표정
            moodSprite.sprite = emotionSprites["sad"];

            Debug.Log("Check Key No");
        }

        Debug.Log($"Set mood sprite to: {moodKey}");
    }

    // Resident의 기분이 바뀔 때 호출하여 업데이트
    public void UpdateMood(string newMood)
    {
        resident.mood = newMood;
        UpdateResidentInfo();
    }

    public Resident GetResident()
    {
        resident.residentName = nameText.text;
        //resident.mood = moodText.text;
        //resident.assets = int.Parse(assetsText.text);
        resident.position = new SerializableVector3(transform.position);
        return resident;
    }
}
