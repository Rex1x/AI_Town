using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

public class ResidentManager : MonoBehaviour
{
    public GameObject residentPrefab; // ResidentPrefab 연결
    //public Transform parentTransform;
    private List<ResidentObjectController> activeResidents = new List<ResidentObjectController>();


    private HashSet<string> existingNames = new HashSet<string>(); // 기존 이름 저장용


    // Sprite 리스트 (Unity Editor에서 할당)
    public Sprite angrySprite;
    public Sprite happySprite;
    public Sprite noEmotionSprite;
    public Sprite normalSprite;
    public Sprite sadSprite;

    public List<Sprite> EmotionSprites = new List<Sprite>();

    private void Start()
    {
        //DataManager.ClearAllResidentData();

        // 저장된 주민 데이터를 불러오고, 각각 화면에 표시
        Resident[] savedResidents = DataManager.LoadAllResidents();
        foreach (Resident savedResident in savedResidents)
        {
            existingNames.Add(savedResident.residentName); // 기존 주민 이름을 저장
            ShowSavedResident(savedResident);
        }
    }

    // 기존 저장된 Resident 데이터를 화면에 표시하는 메서드
    private void ShowSavedResident(Resident residentData)
    {
        Vector3 savedPosition = residentData.position.ToVector3(); // 위치 변환 수정
        GameObject residentObject = Instantiate(residentPrefab, savedPosition, Quaternion.identity, this.transform);
        ResidentObjectController controller = residentObject.GetComponent<ResidentObjectController>();

        // Resident 데이터 초기화
        controller.Initialize(residentData);

        // 기본 표정 스프라이트 설정
        controller.AddEmotionSprite("angry", angrySprite);
        controller.AddEmotionSprite("happy", happySprite);
        controller.AddEmotionSprite("noemotion", noEmotionSprite);
        controller.AddEmotionSprite("normal", normalSprite);
        controller.AddEmotionSprite("sad", sadSprite);

        activeResidents.Add(controller);
    }



    public async Task GenerateAndShowResidents()
    {
        if (activeResidents.Count >= 5)
        {
            Debug.Log("Maximum of 5 residents reached.");
            return;
        }


        
        // 카메라 뷰 내 랜덤한 위치 계산
        Vector3 randomPosition = GetRandomPositionInView();

        Resident newResident = await GenerateUniqueResident(randomPosition);

        GameObject residentObject = Instantiate(residentPrefab, randomPosition, Quaternion.identity, this.transform);
        ResidentObjectController controller = residentObject.GetComponent<ResidentObjectController>();

        // Resident 데이터 초기화
        controller.Initialize(newResident);

        // 기본 표정 스프라이트 설정
        controller.AddEmotionSprite("angry", angrySprite);
        controller.AddEmotionSprite("happy", happySprite);
        controller.AddEmotionSprite("noemotion", noEmotionSprite);
        controller.AddEmotionSprite("normal", normalSprite);
        controller.AddEmotionSprite("sad", sadSprite);

        activeResidents.Add(controller);
        existingNames.Add(newResident.residentName); // 이름을 추가
    }

    private async Task<Resident> GenerateUniqueResident(Vector3 position)
    {
        Resident newResident;
        do
        {
            newResident = await GenerateRandomResident(position);
        } while (existingNames.Contains(newResident.residentName)); // 중복 검사

        return newResident;
    }

    private async Task<Resident> GenerateRandomResident(Vector3 position)
    {
        string prompt = "Create a random villager with a Korean unique name, mood(normal, angry, noemotion, sad, happy), and assets. Format it as: Name: [한글 이름], Mood: [mood], Assets: [number].";
        string apiResponse = await OpenAIAPIHelper.GetGpt4oMiniResponseAsync(prompt);

        Resident newResident = ParseResidentFromAPI(apiResponse, position);
        return newResident;
    }

    private Resident ParseResidentFromAPI(string apiResponse, Vector3 position)
    {

        Debug.Log("Response : " + apiResponse);
        // 정규 표현식을 사용하여 Name, Mood, Assets 추출
        string namePattern = @"Name:\s*(.+?),";
        string moodPattern = @"Mood:\s*(.+?),";
        string assetsPattern = @"Assets:\s*(\d+)";

        string name = Regex.Match(apiResponse, namePattern).Groups[1].Value.Trim();
        string mood = Regex.Match(apiResponse, moodPattern).Groups[1].Value.Trim();
        int assets = int.Parse(Regex.Match(apiResponse, assetsPattern).Groups[1].Value.Trim());

        return new Resident(name, mood, assets, position);
    }

    // 활성화된 Resident 오브젝트의 데이터를 반환
    public Resident[] GetAllResidents()
    {
        if (activeResidents == null || activeResidents.Count == 0)
        {
            Debug.LogWarning("No residents found.");
            return new Resident[0];
        }

        List<Resident> residents = new List<Resident>();
        foreach (var controller in activeResidents)
        {
            residents.Add(controller.GetResident());
        }

        return residents.ToArray();
    }

    private Vector3 GetRandomPositionInView()
    {
        //Camera mainCamera = Camera.main;
        //float x = Random.Range(-mainCamera.orthographicSize * mainCamera.aspect, mainCamera.orthographicSize * mainCamera.aspect);
        //float y = Random.Range(-mainCamera.orthographicSize, mainCamera.orthographicSize);
        float x = Random.Range(-10.5f, 6.8f);
        float y = Random.Range(-4.5f, 2.5f);
        return new Vector3(x, y, 0);
    }


    private void OnApplicationQuit()
    {
        // 게임 종료 시 모든 활성화된 주민의 데이터 저장
        Resident[] residents = GetAllResidents();
        DataManager.SaveAllResidents(residents);
    }

}
