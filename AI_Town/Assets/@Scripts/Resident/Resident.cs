using System.Threading.Tasks;
using UnityEngine;

public class Resident
{
    public int RID { get; set; }

    public string residentName;
    public string mood;
    public float assets;
    public float hungry;
    public SerializableVector3 position; // 위치 정보 추가


    private string currentTask;
    private string cachedResponse; // 캐시된 AI 응답



    public Resident(string name, string mood, float assets, float hungry, Vector3 position)
    {
        this.residentName = name;
        this.mood = mood;
        this.assets = assets;
        this.hungry = hungry;
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

        string prompt = $"{residentName} is currently feeling {mood} and has {assets} foods. What should they do next?";
        string action = await OpenAIAPIHelper.GetGpt4oMiniResponseAsync(prompt);

        // 캐시 응답 저장
        cachedResponse = action;
        currentTask = action;

        Debug.Log($"{residentName}'s next action: {action}");
        return action;
    }


    // AI로부터 행동 결정 요청
    public async Task<string> ReactToGodWordAsync(string godword)
    {
        // 만약 캐시된 응답이 있다면, 그 응답을 사용
        if (!string.IsNullOrEmpty(cachedResponse))
        {
            Debug.Log($"Using cached response for {residentName}: {cachedResponse}");
            return cachedResponse;
        }

        string prompt = $"{residentName} is currently feeling {mood} and has {assets} foods. and God Says '{godword}'. What should they do next?";
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
