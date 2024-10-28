using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class ResidentManager : MonoBehaviour
{
    public GameObject residentPrefab; // ResidentPrefab 연결


    public Collider2D spawnArea; // Resident를 생성할 Collider2D 추가


    //public Transform parentTransform;
    private List<ResidentObjectController> activeResidents = new List<ResidentObjectController>();
    private HashSet<string> usedNames = new HashSet<string>(); // 이미 생성된 Resident 이름 추적


    private HashSet<string> existingNames = new HashSet<string>(); // 기존 이름 저장용
    private HashSet<int> usedRIDs = new HashSet<int>(); // 사용된 RID 저장용
    private const int startingRID = 10000;


    // Sprite 리스트 (Unity Editor에서 할당)
    public Sprite angrySprite;
    public Sprite happySprite;
    public Sprite noEmotionSprite;
    public Sprite normalSprite;
    public Sprite sadSprite;

    public static Dictionary<string, Sprite> emotionSprites = new Dictionary<string, Sprite>();


    // 미리 등록된 16명의 Resident 데이터 배열
    private Resident[] preDefinedResidents = new Resident[]
    {
        new Resident("한결", "normal", 0, 0, "책임감 있고 신중하며 계획적인 성격", "마을에서 가장 조직적인 관리인 역할", "#1D3557", Vector3.zero),
        new Resident("서윤", "normal", 0, 0, "배려심 많고 협력적인 성향", "보호자 역할", "#C5CAE9", Vector3.zero),
        new Resident("지유", "normal", 0, 0, "이상주의적이며 통찰력이 강함", "전략가", "#4A148C", Vector3.zero),
        new Resident("민재", "normal", 0, 0, "논리적이고 독립적", "분석가", "#37474F", Vector3.zero),
        new Resident("준호", "normal", 0, 0, "즉흥적이고 실용적인 성향", "문제 해결사", "#2E7D32", Vector3.zero),
        new Resident("나영", "normal", 0, 0, "감정적이면서도 자유로움", "자유로운 영혼", "#FFAB91", Vector3.zero),
        new Resident("수현", "normal", 0, 0, "공감 능력이 뛰어나고 관계를 중요시", "평화주의자", "#A5D6A7", Vector3.zero),
        new Resident("도현", "normal", 0, 0, "호기심 많고 분석적", "탐구자", "#81D4FA", Vector3.zero),
        new Resident("동훈", "normal", 0, 0, "활동적이고 모험적", "도전자", "#FF7043", Vector3.zero),
        new Resident("미나", "normal", 0, 0, "사교적이고 활기찬 성향", "분위기 메이커", "#F06292", Vector3.zero),
        new Resident("지훈", "normal", 0, 0, "창의적이고 열정적", "혁신가", "#FFEB3B", Vector3.zero),
        new Resident("태우", "normal", 0, 0, "논쟁적이고 창의적", "발명가", "#00ACC1", Vector3.zero),
        new Resident("재호", "normal", 0, 0, "실용적이고 체계적", "관리자", "#D32F2F", Vector3.zero),
        new Resident("은지", "normal", 0, 0, "사교적이고 협력적", "사교적 리더", "#F8BBD0", Vector3.zero),
        new Resident("현서", "normal", 0, 0, "리더십을 발휘하고 협력과 조화를 중시", "중재자", "#303F9F", Vector3.zero),
        new Resident("성민", "normal", 0, 0, "전략적이며 계획적인 리더형", "리더", "#FFD700", Vector3.zero)
    };




    //Location List : 1. 광장 2. 투표실 3. 대화실 4. 대기실

    //행동 List : 1. 악행 2. 선행


    private void Start()
    {

        //** 초기화 **//
        DataManager.ClearAllResidentData();


        SetEmotionSprites();


        // 저장된 주민 데이터를 불러오고, 각각 화면에 표시
        Resident[] savedResidents = DataManager.LoadAllResidents();
        foreach (Resident savedResident in savedResidents)
        {
            existingNames.Add(savedResident.residentName); // 기존 주민 이름을 저장
            usedRIDs.Add(savedResident.RID); // 저장된 Resident의 RID를 사용 목록에 추가

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
        controller.emotionSprites = emotionSprites;

        activeResidents.Add(controller);
    }



    private void SetEmotionSprites()
    {
        emotionSprites.Add("angry", angrySprite);
        emotionSprites.Add("happy", happySprite);
        emotionSprites.Add("noemotion", noEmotionSprite);
        emotionSprites.Add("normal", normalSprite);
        emotionSprites.Add("sad", sadSprite);

    }


    // 새로운 Resident 생성
    public void GenerateRandomResident()
    {
        if (activeResidents.Count >= 16)
        {
            Debug.Log("Maximum of 16 residents reached.");
            return;
        }

        // 미리 정의된 Resident 중에서 중복되지 않는 랜덤 선택
        Resident randomResidentData = GetUniqueRandomResident();
        if (randomResidentData != null)
        {
            CreateResidentFromData(randomResidentData);
        }
        else
        {
            Debug.Log("No more unique residents available.");
        }
    }

    // 중복되지 않는 Resident 데이터 얻기
    private Resident GetUniqueRandomResident()
    {
        List<Resident> availableResidents = new List<Resident>();

        // 사용되지 않은 Resident 필터링
        foreach (var resident in preDefinedResidents)
        {
            if (!usedNames.Contains(resident.residentName))
            {
                availableResidents.Add(resident);
            }
        }

        if (availableResidents.Count == 0) return null;

        // 랜덤하게 하나 선택
        Resident selectedResident = availableResidents[Random.Range(0, availableResidents.Count)];
        usedNames.Add(selectedResident.residentName); // 이름을 사용된 목록에 추가
        return selectedResident;
    }

    // Resident 데이터로부터 오브젝트 생성
    private void CreateResidentFromData(Resident residentData)
    {
        // Collider2D 범위 내에서 랜덤한 위치 생성
        Vector3 randomPosition = GetRandomPositionInCollider(spawnArea);

        // Resident 오브젝트 생성
        GameObject residentObject = Instantiate(residentPrefab, randomPosition, Quaternion.identity, this.transform);
        ResidentObjectController controller = residentObject.GetComponent<ResidentObjectController>();

        // Resident 초기화
        residentData.position = new SerializableVector3(randomPosition);
        controller.Initialize(residentData);
        activeResidents.Add(controller);
    }

    private Vector3 GetRandomPositionInCollider(Collider2D collider)
    {
        Bounds bounds = collider.bounds;
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector3(randomX, randomY, 0);
    }











    //public async Task GenerateAndShowResidents()
    //{
    //    if (activeResidents.Count >= 16)
    //    {
    //        Debug.Log("Maximum of 16 residents reached.");
    //        return;
    //    }


        
    //    // 카메라 뷰 내 랜덤한 위치 계산
    //    Vector3 randomPosition = GetRandomPositionInView();

    //    Resident newResident = await GenerateUniqueResident(randomPosition);

    //    GameObject residentObject = Instantiate(residentPrefab, randomPosition, Quaternion.identity, this.transform);
    //    ResidentObjectController controller = residentObject.GetComponent<ResidentObjectController>();

    //    // Resident 데이터 초기화
    //    controller.Initialize(newResident);
    //    controller.emotionSprites = emotionSprites;

    //    activeResidents.Add(controller);
    //    existingNames.Add(newResident.residentName); // 이름을 추가
    //    usedRIDs.Add(newResident.RID); // 새로운 Resident의 RID 저장




    //}

    //private async Task<Resident> GenerateUniqueResident(Vector3 position)
    //{
    //    Resident newResident;
    //    do
    //    {
    //        newResident = await GenerateRandomResident(position);
    //    } while (existingNames.Contains(newResident.residentName)); // 중복 검사

    //    newResident.RID = GenerateUniqueRID(); // 중복되지 않는 RID 할당
    //    return newResident;
    //}

    private int GenerateUniqueRID()
    {
        int rid = startingRID;

        // 사용되지 않은 RID 찾기
        while (usedRIDs.Contains(rid))
        {
            rid++;
        }

        return rid;
    }

    //private async Task<Resident> GenerateRandomResident(Vector3 position)
    //{
    //    string prompt = "Create a random villager with 1. a unique Korean name, 2. mood(50% : normal, 10% : angry, 20% : noemotion, 10% : sad, 10% : happy), 3. Food. Format it as: Name: [한글 이름], Mood: [mood], Food: [number].";
    //    string apiResponse = await OpenAIAPIHelper.GetGpt4oMiniResponseAsync(prompt);

    //    Resident newResident = ParseResidentFromAPI(apiResponse, position);
    //    return newResident;
    //}

    //private Resident ParseResidentFromAPI(string apiResponse, Vector3 position)
    //{

    //    Debug.Log("Response : " + apiResponse);
    //    // 정규 표현식을 사용하여 Name, Mood, Assets 추출
    //    string namePattern = @"Name:\s*(.+?),";
    //    string moodPattern = @"Mood:\s*(.+?),";
    //    string assetsPattern = @"Food:\s*(\d+)";


    //    string name = Regex.Match(apiResponse, namePattern).Groups[1].Value.Trim();
    //    string mood = Regex.Match(apiResponse, moodPattern).Groups[1].Value.Trim();
    //    int assets = int.Parse(Regex.Match(apiResponse, assetsPattern).Groups[1].Value.Trim());

    //    return new Resident(name, mood, assets, 0, position);
    //}

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



    public bool MoveToLocation()
    {

        //1. 이동할 오브젝트, 이동할 위치
        //2. 이동 성공시 TRUE 리턴
        return true;
    }

    public Vector2 GetResidentLocation(Vector2 position)
    {
        //1. 가져올 오브젝트, 
        //2. 위치 반환

        return position;
    }
}
