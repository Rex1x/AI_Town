using System.Threading.Tasks;
using UnityEngine;

public class Resident
{
    public string residentName;
    public string mood;
    public int assets;
    public SerializableVector3 position; // 위치 정보 추가


    private string currentTask;
    private string cachedResponse; // 캐시된 AI 응답



    public Resident(string name, string mood, int assets, Vector3 position)
    {
        this.residentName = name;
        this.mood = mood;
        this.assets = assets;
        this.position = new SerializableVector3(position);
    }

    // AI로부터 행동 결정 요청
    public async Task<string> DecideNextActionAsync()
    {
        // 만약 캐시된 응답이 있다면, 그 응답을 사용
        if (!string.IsNullOrEmpty(cachedResponse))
        {
            Debug.Log($"Using cached response for {residentName}: {cachedResponse}");
            return cachedResponse;
        }

        string prompt = $"{residentName} is currently feeling {mood} and has {assets} assets. What should they do next?";
        string action = await OpenAIAPIHelper.GetGpt4oMiniResponseAsync(prompt);

        // 캐시 응답 저장
        cachedResponse = action;
        currentTask = action;

        Debug.Log($"{residentName}'s next action: {action}");
        return action;
    }

    // 새로운 상황이 생길 때 캐시를 초기화
    public void ResetCache()
    {
        cachedResponse = null;
    }
}